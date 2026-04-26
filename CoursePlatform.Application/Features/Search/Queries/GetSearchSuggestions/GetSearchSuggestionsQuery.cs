using CoursePlatform.Application.Features.Search.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Search.Queries.GetSearchSuggestions;

public record GetSearchSuggestionsQuery(string Query)
    : IRequest<IReadOnlyList<SearchSuggestionDto>>;