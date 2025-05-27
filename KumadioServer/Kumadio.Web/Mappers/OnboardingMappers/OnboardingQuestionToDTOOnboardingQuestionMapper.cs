using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.DTOS.Auth;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.OnboardingMappers
{
    public class OnboardingQuestionToDTOOnboardingQuestionMapper : DtoMapper<OnboardingQuestion, DTOOnboardingQuestion>
    {
        public override DTOOnboardingQuestion MapCore(OnboardingQuestion source)
        {
            return new DTOOnboardingQuestion
            {
                Key = source.Key,
                Text = source.Text,
                Type = source.Type,
                Options = source.Options
                                .OrderBy(o => o.Order)
                                .Select(o => new DTOOnboardingOption
                                {
                                    Id = o.Id,
                                    Text = o.Text
                                })
                                .ToArray()
            };
        }
    }
}
