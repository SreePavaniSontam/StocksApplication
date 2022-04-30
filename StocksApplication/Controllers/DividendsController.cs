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
	public class DividendsController : Controller
	{
		private readonly IRepository _repository;
		public List<CompanyDividend> detailsOfCompany;
		public string inputSymbol;
		bool isSaved = false;
		public string BASE_URL = "https://cloud.iexapis.com/stable/";
		HttpClient httpClient;

		public DividendsController(IRepository repository)
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
			return RedirectToAction("GetCompanyDividend");
		}

		public IActionResult GetCompanyDividend()
		{
			inputSymbol = Convert.ToString(TempData["value"]);
			detailsOfCompany = GetCompanyDividend(inputSymbol);
			_repository.SaveCompanyLatestDividend(detailsOfCompany);
			return View(detailsOfCompany);
		}

		private List<CompanyDividend> GetCompanyDividend(string symbol)
		{
			List<CompanyDividend> cDividends = new List<CompanyDividend>();
			string CompanyDividends_End_Point = BASE_URL + "stock/" + symbol + "/dividends/2y" + "?token=pk_fb8154e74d144e4c83b0bb8c5a5d294c";
			string apiResponse = string.Empty;
			httpClient.BaseAddress = new Uri(CompanyDividends_End_Point);
			HttpResponseMessage response = httpClient.GetAsync(CompanyDividends_End_Point).GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				apiResponse = apiResponse.Replace(",\"id\":\"DIVIDENDS\"", "");
			}
			if (!string.IsNullOrEmpty(apiResponse))
			{
				cDividends = JsonConvert.DeserializeObject<List<CompanyDividend>>(apiResponse);
			}
			return cDividends;
		}
	}
}