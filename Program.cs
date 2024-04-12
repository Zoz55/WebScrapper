using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;










var AllPics = new List<string>();


if (!File.Exists("pics.txt"))
    File.WriteAllText("pics.txt", "");
else
    AllPics = File.ReadAllLines("pics.txt").ToList();


var AuthURL = "https://www.facebook.com/login.php";
var ProfileURL = "https://web.facebook.com/ScreenMixx/photos";


var options = new ChromeOptions();
options.AddArguments("--disable-notifications");
options.AddArguments("--disable-popup-blocking");
// if you commented the next line you can see the process taking place in the browser 
options.AddArguments("--headless");
var driver = new ChromeDriver(options);
var Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

var LastUrl = "";

if (!File.Exists("url.txt"))
    File.WriteAllText("url.txt", "");
else
    LastUrl = File.ReadAllText("url.txt");



driver.Navigate().GoToUrl(AuthURL);


Wait.Until(driver => driver.FindElement(By.Name("email")).Displayed);
Wait.Until(driver => driver.FindElement(By.Name("email")).Enabled);
// you should write the email to login to facebook in the next line 
driver.FindElement(By.Name("email")).SendKeys("Example@gmail.com");


Wait.Until(driver => driver.FindElement(By.Name("pass")).Displayed);
Wait.Until(driver => driver.FindElement(By.Name("pass")).Enabled);
// you should write the password to login to facebook in the next line 

driver.FindElement(By.Name("pass")).SendKeys("P@ssWord Example");
driver.FindElement(By.Name("pass")).SendKeys(Keys.Enter);

Wait.Until(driver => driver.FindElement(By.TagName("div")).Displayed);


if (string.IsNullOrEmpty(LastUrl))
{

    driver.Navigate().GoToUrl(ProfileURL);
    string xpathb = $"/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[1]/div/div/div[4]/div/div/div/div/div/div/div/div/div[2]/div/div/div/div[2]/a[2]";
    var buttonElement = driver.FindElement(By.XPath(xpathb));
    buttonElement.Click();
    Wait.Until(driver => driver.FindElement(By.TagName("img")).Displayed);

    string xpath = $"/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[1]/div/div/div[4]/div/div/div/div/div/div/div/div/div[3]/div[1]/div[1]/div/div";

    // Find the div element using the constructed XPath
    var divElement = driver.FindElement(By.XPath(xpath));

    // Get the first child element (assuming it's a div)
    var childDivElement = divElement.FindElement(By.XPath(".//*"));

    // Do something with the child div element (e.g., get an element inside it)
    var innerElement = childDivElement.FindElement(By.XPath(".//a"));
    string innerElementSrc = innerElement.GetAttribute("href");
    driver.Navigate().GoToUrl(innerElementSrc);
    //get by css selector of img data-visualcompletion="media-vc-image" ]
    Wait.Until(driver => driver.FindElement(By.CssSelector("img[data-visualcompletion='media-vc-image']")).Displayed);
    var img = driver.FindElement(By.CssSelector("img[data-visualcompletion='media-vc-image']"));
    string imgSrc = img.GetAttribute("src");

    if (!AllPics.Contains(imgSrc))
    {
        AllPics.Add(imgSrc);
        File.AppendAllText("pics.txt", imgSrc + Environment.NewLine);
        Console.WriteLine(imgSrc);
    }
    else
    {
        Console.WriteLine("Already Exists");
    }

    while (true)
    {
        try
        {
            Wait.Until(driver => driver.FindElement(By.CssSelector("div[aria-label='Next photo']")).Displayed);
            Wait.Until(driver => driver.FindElement(By.CssSelector("div[aria-label='Next photo']")).Enabled);
            var nextButton = driver.FindElement(By.CssSelector("div[aria-label='Next photo']"));
            nextButton.Click();
            getImages();
        }
        catch (StaleElementReferenceException ex)
        {
            getImages();
            continue;
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.Message);
            break;
        }
    }

}
else
{
    driver.Navigate().GoToUrl(LastUrl);
    while (true)
    {
        try
        {
            Wait.Until(driver => driver.FindElement(By.CssSelector("div[aria-label='Next photo']")).Displayed);
            Wait.Until(driver => driver.FindElement(By.CssSelector("div[aria-label='Next photo']")).Enabled);
            var nextButton = driver.FindElement(By.CssSelector("div[aria-label='Next photo']"));
            nextButton.Click();
            getImages();
        }
        catch (StaleElementReferenceException ex)
        {
            getImages();
            continue;
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.Message);
            break;
        }
    }

}
void getImages()
{

    Thread.Sleep(2000);
    Wait.Until(driver => driver.FindElement(By.CssSelector("img[data-visualcompletion='media-vc-image']")).Displayed);
    Wait.Until(driver => driver.FindElement(By.CssSelector("img[data-visualcompletion='media-vc-image']")).Enabled);
    var img = driver.FindElement(By.CssSelector("img[data-visualcompletion='media-vc-image']"));
    var imgSrc = img.GetAttribute("src");
    if (!AllPics.Contains(imgSrc))
    {
        AllPics.Add(imgSrc);
        File.AppendAllText("pics.txt", imgSrc + Environment.NewLine);
        Console.WriteLine(imgSrc);
    }
    else
    {
        Console.WriteLine("Already Exists");
    }
    //save url to file
    File.WriteAllText("url.txt", driver.Url);
}

driver.Quit();