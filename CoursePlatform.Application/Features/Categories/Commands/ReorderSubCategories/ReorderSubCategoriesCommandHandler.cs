using CoursePlatform.Application.Common.Exceptions;
using CoursePlatform.Application.Contracts.Persistence;
using CoursePlatform.Application.Contracts.Services;
using CoursePlatform.Application.Features.Categories.Commands.ReorderCategories;
using CoursePlatform.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlatform.Application.Features.Categories.Commands.ReorderSubCategories
{

    public class ReorderSubCategoriesCommandHandler
    : IRequestHandler<ReorderSubCategoriesCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICacheService _cache;

        public ReorderSubCategoriesCommandHandler(
            IUnitOfWork uow,
            ICacheService cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<Unit> Handle(
            ReorderSubCategoriesCommand request, CancellationToken ct)
        {
            foreach (var item in request.Items)
            {
                var sub = await _uow.Repository<SubCategory>()
                                    .GetByIdAsync(item.Id, ct)
                    ?? throw new NotFoundException("SubCategory", item.Id);

                if (sub.CategoryId != request.CategoryId)
                    throw new ForbiddenException(
                        "SubCategory does not belong to this category.");

                sub.Order = item.Order;
                _uow.Repository<SubCategory>().Update(sub);
            }

            await _uow.CompleteAsync(ct);
            await _cache.RemoveAsync("categories:all", ct);
            await _cache.RemoveAsync($"categories:{request.CategoryId}", ct);

            return Unit.Value;
        }

    }
}
