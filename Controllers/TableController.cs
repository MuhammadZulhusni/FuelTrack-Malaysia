using Microsoft.AspNetCore.Mvc;

public class TableController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Table/TableView.cshtml");
    }
}