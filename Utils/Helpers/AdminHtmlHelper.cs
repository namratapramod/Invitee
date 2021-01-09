using Invitee.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Invitee.Utils.Helpers
{
    public class AdminHtmlHelper
    {
        public class TableHeader
        {
            public Dictionary<string, string> TableHeaderInfo { get; set; } = new Dictionary<string, string>();
        }

        public class TableHelper
        {
            public static MvcHtmlString BuildTableHeader(TableHeader tableHeaders, bool enableActionColumn = false)
            {
                var tableHeader = new StringBuilder();
                tableHeader.Append("<thead><tr>");
                foreach (var item in tableHeaders.TableHeaderInfo)
                {
                    tableHeader.Append($"<th {item.Value}>{item.Key}</th>");
                }
                if (enableActionColumn)
                    tableHeader.Append("<th>Action</th>");
                tableHeader.Append("</tr></thead>");

                return new MvcHtmlString(tableHeader.ToString());
            }


            public static MvcHtmlString BuildTableHeader(Type type, string[] excludeColumns = null, string[] extraColumns = null)
            {
                var sb = new StringBuilder();
                var props = type.GetProperties();
                if (excludeColumns != null)
                    props = props.Where(p => !excludeColumns.Contains(p.Name)).ToArray();
                var tableHeaderInfo = props.ToDictionary(x => x.Name, x => "");
                if(extraColumns!=null)
                {
                    foreach (var item in extraColumns)
                    {
                        tableHeaderInfo.Add(item, "");
                    }
                }
                sb.Append(BuildTableHeader(new TableHeader { TableHeaderInfo = tableHeaderInfo }));
                return new MvcHtmlString(sb.ToString());
            }
            public static MvcHtmlString BuildBasicTable<T>(List<T> modelObject, string attributes = "", string[] excludeColumns = null, string editButton = "", bool deleteButton = false)
            {

                var sb = new StringBuilder();
                var props = typeof(T).GetProperties();
                if (excludeColumns != null)
                {
                    props = props.Where(p => !excludeColumns.Contains(p.Name)).ToArray();
                }

                sb.Append($"<table {attributes}>");

                //Build the header
                sb.Append(BuildTableHeader(new TableHeader { TableHeaderInfo = props.ToDictionary(x => x.Name, x => "") },!string.IsNullOrEmpty(editButton) || deleteButton));


                //Build Data
                sb.Append("<tbody>");
                foreach (var item in modelObject)
                {
                    sb.Append("<tr>");
                    foreach (var p in props)
                    {
                        var value = typeof(T).GetProperty(p.Name).GetValue(item);
                        if (p.Name.ToLower() == "id")
                        {
                            sb.Append($"<td class='itemId'>{value}</td>");
                        }
                        else
                        {
                            if (value!=null && value.GetType() == typeof(DateTime))
                                value = ((DateTime)value).ToString("dd-MMM-yyyy");
                            sb.Append($"<td>{value}</td>");
                        }
                    }
                    if (!string.IsNullOrEmpty(editButton) || deleteButton)
                    {
                        sb.Append("<td>");
                        if (!string.IsNullOrEmpty(editButton)) {
                            sb.Append($@"<a class='btn btn-info btn-sm' href='{editButton.Replace("rid", item.GetType().GetProperty("Id").GetValue(item).ToString())}'>
                               <i class='fas fa-pencil-alt'>
                               </i>
                               Edit
                           </a>");
                        }
                        if (deleteButton)
                        {
                            sb.Append($@"&nbsp;<button class='btn btn-danger btn-sm btnDelete'>
                               <i class='fas fa-trash'>
                               </i>
                               Delete
                           </button>");
                        }
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>");
                }
                if (modelObject.Count == 0)
                {
                    sb.Append($"<tr><td align='center' colspan={props.Length}>No Data Available !</td></tr>");
                }
                sb.Append("</tbody></table>");


                return new MvcHtmlString(sb.ToString());
            }
        }

        public static MvcHtmlString BuildInput(string inputType, string nameOrId, string labelString, string placeHolder = "", string classNames = "", string attributes = "")
        {
            var sb = new StringBuilder();
            sb.Append($@"<div class='form-group'>
                        <label for='{nameOrId}'>{labelString}</label>
                        <input type='{inputType}' class='form-control {classNames}' {attributes} name={nameOrId} id='{nameOrId}' placeholder='{placeHolder}'>
                    </div>");
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString BuildTextArea(string nameOrId,int numOfRows, string labelString, string placeHolder = "", string classNames = "", string attributes = "")
        {
            var sb = new StringBuilder();
            sb.Append($@"<div class='form-group'>
                        <label for='{nameOrId}'>{labelString}</label>
                        <textarea numOfRows='{numOfRows}' class='form-control {classNames}' {attributes} name={nameOrId} id='{nameOrId}' placeholder='{placeHolder}'></textarea>
                    </div>");
            return new MvcHtmlString(sb.ToString());
        }
    }

    public static class HtmlExtension
    {
        private static string BootstrapControlTemplate(string labelHtml, string controlHtml)
        {
            return $@"<div class='form-group'>
                          {labelHtml}
                         {controlHtml}
                       </div>";
        }
        public static MvcHtmlString BootStrapTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string labelString, string placeHolder = "", string type = "text")
        {
            var htmlAttributes = new { @class = "form-control", @placeholder = placeHolder, type=type };
            var htmlString = BootstrapControlTemplate(htmlHelper.LabelFor(expression,labelString).ToHtmlString(), htmlHelper.TextBoxFor(expression, htmlAttributes).ToHtmlString());
            return new MvcHtmlString(htmlString);
        }
        public static MvcHtmlString BootStrapTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string labelString, string placeHolder = "", object htmlAttributes = null)
        {
            var htmlString = BootstrapControlTemplate(htmlHelper.LabelFor(expression, labelString).ToHtmlString(), htmlHelper.TextAreaFor(expression, new { @class = "form-control", @placeholder = placeHolder }).ToHtmlString());
            return new MvcHtmlString(htmlString);
        }

        public static JObject GetNavMenu(this HtmlHelper htmlHelper)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(File.ReadAllText(HttpContext.Current.Server.MapPath("~/Views/Shared/Partials/navMenu.json")));
        }
    }
}