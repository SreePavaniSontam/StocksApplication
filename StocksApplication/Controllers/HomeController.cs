using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
	public class HomeController : Controller
	{
		private readonly IRepository _repository;

		public List<Company> companies;
		public CompanyDetails detailsOfCompany;
		public List<CompanyDividend> dividendsOfCompany;
		bool isSaved = false;
		public string BASE_URL = "https://cloud.iexapis.com/stable/";
		HttpClient httpClient;

		public HomeController(IRepository repository)
		{
			_repository = repository;
			httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new
				System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		}

		public IActionResult Companies(string searchBy, string search)
		{
			List<Company> allCompanies = GetAllCompanies();
			bool areNewRcordsInserted = _repository.SaveCompanies(allCompanies);
			if (searchBy == null)
			{
				return View(allCompanies);
			}
			else if (searchBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
			{
				return View(allCompanies.Where(x => x.Symbol.Equals(search, StringComparison.OrdinalIgnoreCase) || search == null).ToList());
			}
			else
			{
				return View(allCompanies.Where(x => x.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase) || search == null).ToList());
			}
		}

		private List<Company> GetAllCompanies()
		{
			string CompaniesApi_End_Point = BASE_URL + "ref-data/symbols?token=pk_fb8154e74d144e4c83b0bb8c5a5d294c";
			string companyList = string.Empty;
			httpClient.BaseAddress = new Uri(CompaniesApi_End_Point);
			HttpResponseMessage response = httpClient.GetAsync(CompaniesApi_End_Point).GetAwaiter().GetResult();
			if (response.IsSuccessStatusCode)
			{
				companyList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			}
			if (!string.IsNullOrEmpty(companyList))
			{
				companies = JsonConvert.DeserializeObject<List<Company>>(companyList);
			}
			return companies;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
