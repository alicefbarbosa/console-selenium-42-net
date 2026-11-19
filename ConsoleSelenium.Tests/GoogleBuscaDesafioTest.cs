using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests;

[TestClass]
public class GoogleBuscaDesafioTest
{
    [TestMethod]
    public void Exercicio1_PesquisaBasica_ValidaResultados()
    {
        using IWebDriver driver = new ChromeDriver();

        try
        {
            driver.Navigate().GoToUrl("https://duckduckgo.com");

            IWebElement campoPesquisa = driver.FindElement(By.Name("q"));

            campoPesquisa.SendKeys("Selenium WebDriver C#");
            campoPesquisa.SendKeys(Keys.Enter);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            wait.Until(d =>
                d.Url.Contains("Selenium", StringComparison.OrdinalIgnoreCase)
            );

            Assert.Contains(
                "Selenium",
                driver.Url,
                "A URL não contém o termo pesquisado."
            );
        }
        finally
        {
            driver.Quit();
        }
    }

    [TestMethod]
    public void Exercicio2_Autocompletar_SelecionarTerceiraSugestao()
    {
        using IWebDriver driver = new ChromeDriver();

        try
        {
            driver.Navigate().GoToUrl("https://duckduckgo.com");

            IWebElement campoPesquisa = driver.FindElement(By.Name("q"));

            campoPesquisa.SendKeys("C# unit testing");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            var sugestoes = wait.Until(d =>
            {
                var elementos = d.FindElements(
                    By.CssSelector(
                        ".search-suggestions-list li, .sbox-suggestions li, [role='option']"
                    )
                );

                return elementos.Count >= 3 ? elementos : null;
            });

            sugestoes[2].Click();

            wait.Until(d => d.Url.Contains("duckduckgo.com"));

            wait.Until(d =>
                d.FindElements(By.CssSelector("[data-testid='result']"))
                    .Count > 0
            );

            Assert.Contains(
                "duckduckgo.com",
                driver.Url
            );
        }
        finally
        {
            driver.Quit();
        }
    }

    [TestMethod]
    public void Exercicio3_NavegacaoHistorico_VoltarEAvancar()
    {
        using IWebDriver driver = new ChromeDriver();

        try
        {
            // Arrange - Acessar DuckDuckGo
            driver.Navigate().GoToUrl("https://duckduckgo.com");

            // Act - Realizar pesquisa
            IWebElement campoPesquisa = driver.FindElement(By.Name("q"));

            campoPesquisa.SendKeys("Automated Testing");
            campoPesquisa.SendKeys(Keys.Enter);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Aguardar resultados orgânicos
            var resultados = wait.Until(d =>
            {
                var elementos = d.FindElements(
                    By.CssSelector("a[data-testid='result-title-a']")
                );

                return elementos.Count > 0 ? elementos : null;
            });

            // Guardar URL dos resultados
            string urlResultados = driver.Url;

            // Selecionar primeiro resultado
            resultados[0].Click();

            // Aguardar carregamento da página externa
            wait.Until(d =>
                !d.Url.Contains("duckduckgo.com")
            );

            // Assert - Validar página externa
            Assert.IsFalse(
                driver.Url.Contains("duckduckgo.com"),
                "A navegação para uma página externa não aconteceu."
            );

            // Act - Voltar para os resultados
            driver.Navigate().Back();

            // Aguardar restauração dos resultados
            wait.Until(d =>
                d.Url == urlResultados &&
                d.FindElements(
                    By.CssSelector("a[data-testid='result-title-a']")
                ).Count > 0
            );

            // Assert - Validar retorno
            Assert.AreEqual(
                urlResultados,
                driver.Url,
                "A página de resultados não foi restaurada corretamente."
            );
        }
        finally
        {
            driver.Quit();
        }
    }
}