using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StocksApplication.Models;

namespace StocksApplication.Views
{
	public class QuoteController : Controller
	{
		private readonly IRepository _repository;
		public CompanyQuote detailsOfCompany;
		public string inputSymbol;
		bool isSaved = false;
		public string BASE_URL = "https://cloud.iexapis.com/stable/";
		HttpClient httpClient;

		public QuoteController(IRepository repository)
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
			return RedirectToAction("GetCompanyQuote");
		}
		public IActionResult GetCompanyQuote()
		{
			inputSymbol = Convert.ToString(TempData["value"]);
			detailsOfCompany = GetCompanyQuote(inputSymbol);
			if (detailsOfCompany.Symbol == null)
			{
				ViewBag.IsModelEmpty = true;
			}
			else
				ViewBag.IsModelEmpty = false;
			_repository.SaveCompanyQuote(detailsOfCompany);
			return View(detailsOfCompany);
		}


		private CompanyQuote GetCompanyQuote(string symbol)
		{
			CompanyQuote cQ = new CompanyQuote();
			string CompanyQuote_End_Point = BASE_URL + "stock/" + symbol + "/quote" + "?token=pk_fb8154e74d144e4c83b0bb8c5a5d294c";
			string apiResponse = string.Empty;
			httpClient.BaseAddress = new Uri(CompanyQuote_End_Point);
			HttpResponseMessage response = httpClient.GetAsync(CompanyQuote_End_Point).GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			}
			if (!string.IsNullOrEmpty(apiResponse))
			{
				cQ = JsonConvert.DeserializeObject<CompanyQuote>(apiResponse);
			}
			return cQ;
		}
	}
}