using System.Text.RegularExpressions;

namespace CoursePlatform.Application.Common.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string input)
    {
        var slug = input.ToLowerInvariant().Trim();

        slug = slug.Replace("&", "and").Replace("@", "at");
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");

        return slug.Trim('-');
    }

   
    public static async Task<string> GenerateUniqueSlugAsync(
        string input,
        Func<string, Task<bool>> slugExistsAsync)
    {
        var baseSlug = GenerateSlug(input);

        if (!await slugExistsAsync(baseSlug))
            return baseSlug;

        var counter = 2;
        string candidate;

        do
        {
            candidate = $"{baseSlug}-{counter}";
            counter++;
        }
        while (await slugExistsAsync(candidate));

        return candidate;
    }
}