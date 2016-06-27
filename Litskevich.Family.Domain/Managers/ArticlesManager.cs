using Litskevich.Family.Domain.Contracts;
using SXCore.Common.Contracts;
using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.Domain.Entities;
using SXCore.Common.Values;
using System;
using System.Collections.Generic;
using SXCore.Common.Exceptions;
using Litskevich.Family.Domain.Values;
using System.Linq;
using SXCore.Common.Services;
using SXCore.Common.Interfaces;
using SXCore.Domain.Entities;
using SXCore.Common.Classes;
using Litskevich.Family.Domain.Events;
using Litskevich.Family.Domain.Contracts.Services;

namespace Litskevich.Family.Domain.Managers
{
    public class ArticlesManager : BaseManager, IArticlesManager
    {
        public ArticlesManager(IFamilyUnitOfWork uow, IFamilyInfrastructureProvider infrastructure, ITokenProvider tokenProvider)
            : base(uow, infrastructure, tokenProvider)
        { }

        public Article GetArticle(long id)
        {
            return this.UnitOfWork.ArticleRepository.Get(id);
        }

        public Article GetArticleWithMaterials(long id)
        {
            return this.UnitOfWork.ArticleRepository.GetWithMaterials(id);
        }

        public IEnumerable<Article> GetArticles(string filter = "", int pageNumber = 0, int pageSize = Article.DefaultPageSize)
        {
            return this.UnitOfWork.ArticleRepository.FindAll(filter, pageNumber, pageSize);
        }

        public IEnumerable<Article> GetArticles(long personID, int pageNumber = 0, int pageSize = Article.DefaultPageSize)
        {
            return this.UnitOfWork.ArticleRepository.FindByPerson(personID, pageNumber, pageSize);
        }

        public Article CreateArticle(string title, DateTime? periodBegin = null, DateTime? periodEnd = null, string comment = "")
        {
            var article = Article.Create(CommonService.Now, this.CurrentPerson, title, periodBegin, periodEnd, comment ?? "");

            this.UnitOfWork.ArticleRepository.Add(article);

            this.SaveChanges();

            return article;
        }

        public Article UpdateArticle(long articleID, string title, DateTime? periodBegin = null, DateTime? periodEnd = null, string comment = "", long[] personIDs = null)
        {
            var article = this.GetArticle(articleID);
            if (article == null)
                throw new CustomNotFoundException($"Article with ID={articleID} not found!");

            // change article's info
            article.Change(title, periodBegin, periodEnd, comment);

            // change members
            if (personIDs != null)
                article.ChangeMembers(this.UnitOfWork.PersonRepository.GetByIDs(personIDs));

            this.UnitOfWork.ArticleRepository.Update(article);

            this.SaveChanges();

            return article;
        }

        public Article ImportArticle(DateTimeOffset created, string title, string comment, DateTime? periodBegin, DateTime? periodEnd, IEnumerable<long> members)
        {
            var author = this.UnitOfWork.PersonRepository.GetAll().First();

            var article = Article.Create(created, author, title, periodBegin, periodEnd, comment ?? "");

            article.ChangeMembers(this.UnitOfWork.PersonRepository.GetByIDs(members));

            this.UnitOfWork.ArticleRepository.Add(article);

            this.SaveChanges();

            return article;
        }
    }
}
