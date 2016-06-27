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

        protected Token BuildToken(ManagerToken managerToken)
        {
            var now = CommonService.Now;

            // проверяем токен на валидность и годность
            if (managerToken == null || now < managerToken.Date || managerToken.Expire < now)
                return null;

            // берем менеджера из токена
            var manager = managerToken?.Manager;
            if (manager == null)
                return null;

            // берем пользователя из токена и проверяем на активность (не удален)
            var person = manager?.Person;
            if (person == null)
                return null;

            // создаем стандартный токен
            return new Token(0, person.ID, manager.ID, manager.Login, managerToken.Code, managerToken.Expire, person.Name, person.Avatar)
            {
                Roles = manager.Roles.Select(r => r.Name).ToArray()
            };
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
                var managerToken = dbContext.Set<ManagerToken>()
                .Include(t => t.Manager.Roles)
                .Include(t => t.Manager.Person.Avatar)
                //.Where(t => t.Manager.Person.State == Common.Enums.ObjectState.Active)
                .SingleOrDefault(t => t.Code.ToLower() == token.ToLower());

                if (managerToken == null)
                    return null;

                result = this.BuildToken(managerToken);

                //result = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(managerToken.Data);
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
                // находим пользователя по логину
                var manager = dbContext.Set<Manager>()
                .Include(t => t.Person.Avatar)
                .Include(t => t.Roles)
                //.Where(m => m.Person.State == Common.Enums.ObjectState.Active)
                .FirstOrDefault(m => m.Login.ToLower() == login.ToLower());

                if (manager == null || !CommonService.VerifyPassword(password, manager.Password))
                    return null;

                //result = this.CreateToken(manager);
                //if (result == null)
                //    return null;


                //DateTimeOffset expire = result?.Expire == null ? CommonService.Now.AddHours(DefaultExpirationHours) : (DateTimeOffset)result?.Expire.Value;
                DateTimeOffset expire = now.AddHours(DefaultExpirationHours);

                //var managerToken = ManagerToken.Create(result.Key, manager, expire, Newtonsoft.Json.JsonConvert.SerializeObject(result));
                var managerToken = ManagerToken.Create(Guid.NewGuid().ToString(), manager, expire, "");

                dbContext.Set<ManagerToken>().Add(managerToken);
                dbContext.SaveChanges();

                result = this.BuildToken(managerToken);
            }

            return result;
        }
    }
}
