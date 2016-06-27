using Litskevich.Family.Domain.Contracts.Managers;
using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.Domain.Values;
using Litskevich.Family.WebApi.Models;
using Litskevich.Family.WebApi.Models.Requests;
using SXCore.Common;
using SXCore.Common.Contracts;
using SXCore.Common.Exceptions;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.WebApi;
using SXCore.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Litskevich.Family.WebApi.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/article")]
    public class ArticleApiController : CoreApiController
    {
        private IArticlesManager _manager;

        public ArticleApiController(IArticlesManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<ArticleModel> GetAll(string filter = "", int pageNumber = 0, int pageSize = 20)
        {
            return _manager.GetArticles(filter, pageNumber, pageSize).Select(a => a.ToSerial());
        }

        [HttpGet]
        [Route("~/api/person/{personID:long}/articles")]
        public IEnumerable<ArticleModel> GetAllByPerson(long personID, int pageNumber = 0, int pageSize = 20)
        {
            return _manager.GetArticles(personID, pageNumber, pageSize).Select(a => a.ToSerial());
        }

        [HttpGet]
        [Route("{id:long}")]
        public ArticleModel Get(long id)
        {
            var article = _manager.GetArticleWithMaterials(id);
            if (article == null)
                throw new CustomNotFoundException($"Article with ID={id} not found!");

            return article.ToSerial();
        }

        [ApiAuthorize(Roles = "Supervisor,Admin")]
        [HttpPost]
        [Route("")]
        public ArticleModel CreateArticle([FromBody]CreateArticleRequest request)
        {
            var article = _manager.CreateArticle(request.Title);
            return article?.ToSerial();
        }

        [ApiAuthorize(Roles = "Supervisor,Admin")]
        [HttpPost]
        [Route("{id:long}")]
        public ArticleModel UpdateArticle(long id, [FromBody]UpdateArticleRequest request)
        {
            var article = _manager.UpdateArticle(id, request.Title, request.PeriodBegin, request.PeriodEnd, request.Comment, request.Persons);
            return article?.ToSerial();
        }
    }
}