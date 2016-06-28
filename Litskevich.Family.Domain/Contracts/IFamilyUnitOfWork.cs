﻿using Litskevich.Family.Domain.Contracts.Repositories;
using SXCore.Domain.Contracts;

namespace Litskevich.Family.Domain.Contracts
{
    public interface IFamilyUnitOfWork : ICoreUnitOfWork
    {
        IPersonRepository PersonRepository { get; }
        IGuestRepository GuestRepository { get; }
        IArticleRepository ArticleRepository { get; }
        IMaterialRepository MaterialRepository { get; }
    }
}
