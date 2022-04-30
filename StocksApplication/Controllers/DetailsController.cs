using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
	public class DetailsController : Controller
	{
		private readonly IRepository _repository;
		public CompanyDetails detailsOfCompany;
		public CompanyDetails tempDetails;
		public string inputSymbol;
		bool isSaved = false;
		public string BASE_URL = "https://cloud.iexapis.com/stable/";
		HttpClient httpClient;

		public DetailsController(IRepository repository)
		{
			_repository = repository;
			httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new
				System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		}
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public IActionResult Index(string symbol)
		{
			TempData["value"] = symbol;
			return RedirectToAction("GetCompanyInfo");
		}

		public IActionResult GetCompanyInfo()
		{
			inputSymbol = Convert.ToString(TempData["value"]);
			detailsOfCompany = GetCompanyDetails(inputSymbol);
			if (detailsOfCompany.Symbol == null)
			{
				ViewBag.IsModelEmpty = true;
			}
			else
				ViewBag.IsModelEmpty = false;
			_repository.SaveCompanyDetails(detailsOfCompany);
			return View(detailsOfCompany);
		}

		private CompanyDetails GetCompanyDetails(string symbol)
		{
			CompanyDetails cD = new CompanyDetails();
			string CompanyDetails_End_Point = BASE_URL + "stock/" + symbol + "/company" + "?token=pk_fb8154e74d144e4c83b0bb8c5a5d294c";
			string apiResponse = string.Empty;
			httpClient.BaseAddress = new Uri(CompanyDetails_End_Point);
			HttpResponseMessage response = httpClient.GetAsync(CompanyDetails_End_Point).GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			}
			if (!string.IsNullOrEmpty(apiResponse))
			{
				cD = JsonConvert.DeserializeObject<CompanyDetails>(apiResponse);
			}
			return cD;
		}

	}
}