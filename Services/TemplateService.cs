namespace hoohub.Services
{
    /// <summary>
    /// Handles email templating services.
    /// </summary>
    public static class TemplateService
    {
        /// <summary>
        /// Given a email template string, performs all the given substitutions and returns the template.
        /// </summary>
        /// <param name="template">The template to perform substitutions on.</param>
        /// <param name="substitutions">The <see cref="Dictionary{string, string}"/> substitions to make in the template.</param>
        /// <returns></returns>
        public static string GetTemplateSubstitutions(string template, Dictionary<string, string> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                template = template.Replace(substitution.Key, substitution.Value);
            }
            return template;
        }
    }
}