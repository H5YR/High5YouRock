using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace H5YR.Core.ViewComponents
{
    public class DiscourseReactionsViewComponent : ViewComponent
    {
        private readonly IDiscourseService _discourseService;

        public DiscourseReactionsViewComponent(IDiscourseService discourseService)
        {
            _discourseService = discourseService;
        }

        public IViewComponentResult Invoke(int count = 10)
        {
            var reactions = _discourseService.GetRecentReactions(count);
            return View(reactions);
        }
    }
}