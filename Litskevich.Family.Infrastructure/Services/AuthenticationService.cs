using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Services;
using Litskevich.Family.Infrastructure.Data;
using SXCore.Common.Contracts;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Infrastructure.Services;
using SXCore.Infrastructure.Services.FileStorage;
using SXCore.Infrastructure.Values;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace Litskevich.Family.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationProvider
    {
        public const int DefaultExpirationHours = 4;

        private IFamilyInfrastructureProvider _infrastructure;

        public AuthenticationService(IFamilyInfrastructureProvider infrastructure)
        {
            _infrastructure = infrastructure;
        }

        protected Token BuildToken(UserToken userToken)
        {
            var now = CommonService.Now;

            // проверяем токен на валидность и годность
            if (userToken == null || now < userToken.Date || userToken.Expire < now)
                return null;

            if (userToken.Manager != null)
            {
                var manager = userToken.Manager;

                var person = manager.Person;
                if (person == null)
                    return null;

                return new Token(0, person.ID, manager.ID, manager.Login, userToken.Code, userToken.Expire, person.Name, person.Avatar)
                {
                    Roles = manager.Roles?.Select(r => r.Name).ToArray()
                };
            }
            else if (userToken.Guest != null)
            {
                var guest = userToken.Guest;

                return new Token(0, 0, 0, guest.Login, userToken.Code, userToken.Expire, guest.Name)
                {
                    Roles = new string[] { "User" }
                };
            }

            return null;
        }

        //protected Token CreateToken(Manager manager, int hours = DefaultExpirationHours)
        //{
        //    var now = CommonService.Now;

        //    if (manager == null)
        //        return null;

        //    // берем пользователя из токена и проверяем на активность (не удален)
        //    var person = manager?.Person;
        //    if (person == null)
        //        return null;

        //    var key = Guid.NewGuid().ToString();

        //    // создаем стандартный токен
        //    var token = new Token(0, person.ID, manager.ID, manager.Login, key, now.AddHours(DefaultExpirationHours), person.Name, person.Avatar)
        //    {
        //        Roles = manager.Roles.Select(r => r.Name).ToArray()
        //    };

        //    // создаем доступ к Azure для просмотра материалов
        //    var sas = this.CreateSASToken();
        //    if (!String.IsNullOrWhiteSpace(sas))
        //        token.Values.Add(MaterialDataProvider.StorageSASTokenName, sas);

        //    return token;
        //}

        public Token Authenticate(string token)
        {
            // authentication by the Token
            var now = CommonService.Now;

            Token result = null;

            // создаем новый контекст, дабы не использовать "уже загруженные" сущности
            using (var dbContext = new FamilyDbContext())
            {
                // находим токин пользователя в базе по заданному коду токена
                var managerToken = dbContext.Set<UserToken>()
                .Include(t => t.Manager.Roles)
                .Include(t => t.Manager.Person.Avatar)
                .Include(t => t.Guest)
                //.Where(t => t.Manager.Person.State == Common.Enums.ObjectState.Active)
                .SingleOrDefault(t => t.Date <= now && now <= t.Expire && t.Code.ToLower() == token.ToLower());

                if (managerToken == null)
                    return null;

                result = this.BuildToken(managerToken);
            }

            return result;
        }

        public Token Authenticate(string login, string password)
        {
            // authentication by the Login and Password
            var now = CommonService.Now;

            Token result = null;

            // создаем новый контекст, дабы не использовать "уже загруженные" сущности
            using (var dbContext = new FamilyDbContext())
            {
                UserToken userToken = null;

                // authentication for Managers
                if (userToken == null)
                {
                    // находим пользователя по логину
                    var manager = dbContext.Set<Manager>()
                    .Include(t => t.Person.Avatar)
                    .Include(t => t.Roles)
                    //.Where(m => m.Person.State == Common.Enums.ObjectState.Active)
                    .FirstOrDefault(m => m.Login.ToLower() == login.ToLower());

                    if (manager != null && CommonService.VerifyPassword(password, manager.Password))
                        userToken = UserToken.Create(manager, null, now.AddHours(DefaultExpirationHours));
                }

                // authentication for Guests
                if (userToken == null)
                {
                    var guest = dbContext.Set<Guest>()
                        .FirstOrDefault(g => g.Date <= now && now <= g.Expire && g.Login.ToLower() == login.ToLower());

                    if (guest != null && CommonService.VerifyPassword(password, guest.Password))
                        userToken = UserToken.Create(null, guest, guest.Expire);
                }

                // saving new token 
                if (userToken != null)
                {
                    dbContext.Set<UserToken>().Add(userToken);
                    dbContext.SaveChanges();

                    result = this.BuildToken(userToken);
                }
            }

            return result;
        }
    }
}
