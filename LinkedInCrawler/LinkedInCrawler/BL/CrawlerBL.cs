using HtmlAgilityPack;
using LinkedInCrawler.DAL;
using LinkedInCrawler.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;

namespace LinkedInCrawler.BL
{
    public class CrawlerBL
    {
        private CrawlerDAL _dal;
        public CrawlerBL()
        {
            _dal = new CrawlerDAL();
        }

        public User ParseURL(string url)
        {
            string htmlContent = null;
            using (WebClient client = new WebClient())
            {
                htmlContent = client.DownloadString(url);
            }
            User user = GenerateUser(htmlContent);
            return user;
        }

        private User GenerateUser(string htmlContent)
        {
            List<string> skills = null;
            User user = new User();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(htmlContent);
            user.Name = htmlDoc.DocumentNode.SelectNodes("//h1[@id='name']").Select(x => x.InnerText.Trim()).First();
            user.Title = htmlDoc.DocumentNode.SelectNodes("//p[@class='headline title']").Select(x => x.InnerText.Trim()).First();
            user.CurrentPosition = htmlDoc.DocumentNode.SelectNodes("//tr[@data-section='currentPositionsDetails']//span[@class='org']//a").Select(x => x.InnerText.Trim()).First();
            
            var summaryNode = htmlDoc.DocumentNode.SelectNodes("//div[@class='description']//p");
            if (summaryNode != null)
            {
                user.Summary = summaryNode.Select(x => x.InnerText.Trim()).First();
            }
            var topSkills = htmlDoc.DocumentNode.SelectNodes("//div[@class='profile-skills']//ul[@class='skills-section edit-action-area']//li");
            if (topSkills != null)
            {
                user.TopSkillsCounter = topSkills.Count;
                skills = topSkills.Select(x => x.InnerText.Trim()).ToList();
            }
            if (skills == null)
            {
                skills = new List<string>();
            }
            var moreSkills = htmlDoc.DocumentNode.SelectNodes("//ul[@class='skills-section compact-view field-text']//li[@class='endorse-item has-endorsements']//span[@class='endorse-item-name-text']");
            if (moreSkills != null) ;
            {
                var moreSkillsList = moreSkills.Select(x => x.InnerText.Trim()).ToList();
                skills.AddRange(moreSkillsList);
            }
            user.Skills = skills;
            return user;
        }

        internal User GetUser(string name)
        {
            DataTable tbl = _dal.GetUser(name);
            if (tbl.Rows.Count == 0)
                return null;
            DataRow userRow = _dal.GetUser(name).Rows[0];
            User user = new User();
            user.ID = int.Parse(userRow["ID"] as string);
            user.Name = userRow["Name"] as string;
            user.Title = userRow["Title"] as string;
            user.CurrentPosition = userRow["Current_Position"] as string;
            user.Summary = userRow["Summary"] as string;
            user.TopSkillsCounter = int.Parse(userRow["Top_Skills_Counter"] as string);
            DataTable dt = _dal.GetSkills(user.ID);
            user.Skills = dt.AsEnumerable().Select(x => x.Field<string>("Name")).ToList();

            return user; 
        }

        public void SaveUser(User user)
        {
            _dal.saveUser(user);
        }
    }
}