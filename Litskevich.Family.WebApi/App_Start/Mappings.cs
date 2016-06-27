using Litskevich.Family.Domain.Contracts.Services;
using Litskevich.Family.Domain.Entities;
using Litskevich.Family.WebApi.Models;
using SXCore.Common.Contracts;
using SXCore.Domain.Entities;
using System.Linq;

namespace Litskevich.Family.WebApi
{
    public static class Mappings
    {
        //static public AvatarModel ToSerial(this Avatar avatar)
        //{
        //    if (avatar == null)
        //        return null;

        //    return new AvatarModel(avatar.Code);
        //}

        static public PersonModel ToSerial(this Person person)
        {
            if (person == null)
                return null;

            return new PersonModel()
            {
                ID = person.ID,
                Avatar = person.Avatar?.Code,
                Gender = person.Gender,
                Name = person.Name,
                Email = person.Email,
                Phone = person.Phone,
                DateBirth = person.DateBirth,
                DateDeath = person.DateDeath
            };
        }

        //static public MemberModel ToSerialMember(this Person person)
        //{
        //    if (person == null)
        //        return null;

        //    return new MemberModel()
        //    {
        //        ID = person.ID,
        //        Avatar = person.Avatar?.Code,
        //        Name = $"{person.Name.Last} {person.Name.First} {person.Name.Second}"
        //    };
        //}

        //static public ManagerModel ToSerial(this Manager manager)
        //{
        //    if (manager == null)
        //        return null;

        //    return new ManagerModel()
        //    {
        //        Login = manager.Login
        //    };
        //}

        static public MaterialModel ToSerial(this Material material)
        {
            if (material == null)
                return null;

            return new MaterialModel()
            {
                ID = material.ID,
                Url = material.GetPath(),
                FileName = material.File.FileName,
                Type = material.FileType.ToString(),
                Date = material.Date,
                Title = material.Title,
                Comment = material.Comment
            };
        }

        static public ArticleModel ToSerial(this Article article)
        {
            if (article == null)
                return null;

            return new ArticleModel()
            {
                ID = article.ID,
                Author = article.Author == null ? null : new AuthorModel(article.Author.ID, article.Author.Name.ToString("last first second"), article.Author.Avatar),
                Date = article.Date,
                PeriodBegin = article.PeriodBegin,
                PeriodEnd = article.PeriodEnd,
                Title = article.Title,
                Comment = article.Comment, 
                Members = article.Members?.Select(p => new MemberModel(p.ID, p.Name.ToString("last first second"), p.Avatar?.Code)).ToList(),
                Materials = article.Materials?.Select(m => m.ToSerial()).ToList()
            };
        }

        static public FileBlobModel ToSerial(this FileBlob blob)
        {
            if (blob == null)
                return null;

            return new FileBlobModel()
            {
                //FileID = blob.ID,
                Code = blob.Code,
                FileName = blob.FileName,
                Size = blob.Size
            };
        }
    }
}
