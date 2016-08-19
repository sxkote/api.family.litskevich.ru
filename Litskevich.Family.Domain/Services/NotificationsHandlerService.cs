using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Events;
using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Interfaces;
using SXCore.Common.Values;
using System;
using System.Text;

namespace Litskevich.Family.Domain.Services
{
    public class NotificationsHandlerService :
        IDomainEventHandler<RegistrationRequestedEvent>,
        IDomainEventHandler<ManagerCreatedEvent>,
        IDomainEventHandler<GuestCreatedEvent>,
        IDomainEventHandler<PasswordRecoveryRequestedEvent>,
        IDomainEventHandler<PasswordChangedEvent>
    {
        private IFamilyInfrastructureProvider _infrastructure;

        public IFamilyInfrastructureProvider Infrastructure { get { return _infrastructure; } }

        public INotificationService EmailService { get { return _infrastructure?.EmailService; } }

        public NotificationsHandlerService(IFamilyInfrastructureProvider infrastructure)
        {
            _infrastructure = infrastructure;

            if (this.Infrastructure == null || this.EmailService == null)
                throw new CustomOperationException("Email Service is not specified in web.config settings!");
        }

        protected string GetAddress(INotificationService service, string email, string phone)
        {
            if (service == null)
                return null;

            if (service is ISmsNotificationService)
                return phone ?? "";
            else if (service is IEmailNotificationService)
                return email ?? "";

            return null;
        }

        public void Handle(RegistrationRequestedEvent args)
        {
            var sb = new StringBuilder();
            sb.AppendLine("На сайте семейного архива был сформирован запрос на регистрацию нового пользователя:");
            sb.AppendLine($"Фамилия: {args.Name.Last}");
            if (!String.IsNullOrWhiteSpace(args.Name.Maiden)) sb.AppendLine($"Девичья фамилия: {args.Name.Maiden}");
            sb.AppendLine($"Имя: {args.Name.First}");
            sb.AppendLine($"Отчество: {args.Name.Second}");
            sb.AppendLine($"Email: {args.Email}");
            sb.AppendLine($"Телефон: {args.Phone}");
            sb.AppendLine($"Коментарий: {args.Comment}");
            sb.AppendLine();
            sb.AppendLine("С уважением,");
            sb.AppendLine("Семейный Архив Лицкевичей");

            var email = this.Infrastructure.MainEmail;
            if (!String.IsNullOrWhiteSpace(email))
                this.EmailService.SendNotification(email, new Message("Регистрация в семейном архиве", sb.ToString()));
        }

        public void Handle(ManagerCreatedEvent args)
        {
            if (String.IsNullOrWhiteSpace(args.Email))
                return;

            var sb = new StringBuilder();
            sb.AppendLine("Добро пожаловать на сайт семейного архива:");
            sb.AppendLine();
            sb.AppendLine($"Адрес сайта: {this.Infrastructure.WebSiteUrl}");
            sb.AppendLine($"Логин: {args.Login}");
            sb.AppendLine($"Пароль: {args.Password}");
            sb.AppendLine();
            sb.AppendLine("С уважением,");
            sb.AppendLine("Семейный Архив Лицкевичей");

            try { this.EmailService.SendNotification(args.Email, new Message("Регистрация в семейном архиве", sb.ToString())); }
            catch { }
        }

        public void Handle(GuestCreatedEvent args)
        {
            if (String.IsNullOrWhiteSpace(args.Email))
                return;

            var sb = new StringBuilder();
            sb.AppendLine("Вы были приглашены для просмотра семейного сайта Архив Лицкевичей:");
            sb.AppendLine();
            sb.AppendLine($"Адрес сайта: {this.Infrastructure.WebSiteUrl}");
            sb.AppendLine($"Логин: {args.Login}");
            sb.AppendLine($"Пароль: {args.Password}");
            sb.AppendLine();
            sb.AppendLine("С уважением,");
            sb.AppendLine("Семейный Архив Лицкевичей");

            try { this.EmailService.SendNotification(args.Email, new Message("Приглашение в Архив Лицкевичей", sb.ToString())); }
            catch { }
        }

        public void Handle(PasswordRecoveryRequestedEvent args)
        {
            if (String.IsNullOrWhiteSpace(args.Email))
                return;

            var sb = new StringBuilder();
            sb.AppendLine($"Вы создали запрос на восстановление пароля на сайте семейного ахрива:");
            sb.AppendLine($"{this.Infrastructure.WebSiteUrl}");
            sb.AppendLine();
            sb.AppendLine($"Если Вы этого не делали, просто проигнорируйте это письмо!");
            sb.AppendLine();
            sb.AppendLine($"Если, все же, это необходимо Вам, то для восстановления пароля, перейдите по этой ссылке:");
            sb.AppendLine($"{this.Infrastructure.PasswordRecoveryPage}{args.Code}");
            sb.AppendLine();
            sb.AppendLine("С уважением,");
            sb.AppendLine("Семейный Архив Лицкевичей");

            try { this.EmailService.SendNotification(args.Email, new Message("Восстановление пароля на сайте семейного архива", sb.ToString())); }
            catch { }
        }

        public void Handle(PasswordChangedEvent args)
        {
            if (String.IsNullOrWhiteSpace(args.Email))
                return;

            var sb = new StringBuilder();
            sb.AppendLine("Ваш пароль для сайта семейного архива был изменен:");
            sb.AppendLine();
            sb.AppendLine($"Адрес сайта: {this.Infrastructure.WebSiteUrl}");
            sb.AppendLine($"Логин: {args.Login}");
            sb.AppendLine($"Пароль: {args.Password}");
            sb.AppendLine();
            sb.AppendLine("С уважением,");
            sb.AppendLine("Семейный Архив Лицкевичей");

            try { this.EmailService.SendNotification(args.Email, new Message("Смена пароля на сайте семейного архива", sb.ToString())); }
            catch { }
        }
    }
}
