using Microsoft.AspNetCore.Mvc;

namespace Eileen.Components
{
    [ViewComponent(Name = "Eileen.PagedResultsNavigation")]
    public class PagedResultsNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(dynamic viewModel)
        {
            return View(viewModel);
        }   
    }
}