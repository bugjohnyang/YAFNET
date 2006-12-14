/* Yet Another Forum.net
 * Copyright (C) 2003 Bj�rnar Henden
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for cp_subscriptions.
	/// </summary>
	public partial class cp_subscriptions : ForumPage
	{

		public cp_subscriptions() : base("CP_SUBSCRIPTIONS")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(User==null)
			{
				if(CanLogin)
					Forum.Redirect( ForumPages.login,"ReturnUrl={0}",Utils.GetSafeRawUrl());
				else
					Forum.Redirect( ForumPages.forum);
			}
			
			if(!IsPostBack) 
			{
				BindData();

				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink( ForumPages.forum));
				PageLinks.AddLink(PageUserName,Forum.GetLink( ForumPages.cp_profile));
				PageLinks.AddLink(GetText("TITLE"),"");

				UnsubscribeForums.Text = GetText("unsubscribe");
				UnsubscribeTopics.Text = GetText("unsubscribe");
			}
		}

		private void BindData() {
			ForumList.DataSource = YAF.Classes.Data.DB.watchforum_list(PageUserID);
			TopicList.DataSource = YAF.Classes.Data.DB.watchtopic_list(PageUserID);
			DataBind();
		}

		protected string FormatForumReplies(object o) 
		{
			DataRowView row = (DataRowView)o;
			return String.Format("{0}",(int)row["Messages"] - (int)row["Topics"]);
		}

		protected string FormatLastPosted(object o) {
			DataRowView row = (DataRowView)o;

			if(row["LastPosted"].ToString().Length==0)
				return "&nbsp;";

			string link = String.Format("<a href=\"{0}\">{1}</a>",
				Forum.GetLink( ForumPages.profile,"u={0}",row["LastUserID"]),
				row["LastUserName"]
			);
			string by = String.Format(GetText("lastpostlink"),
				FormatDateTime((DateTime)row["LastPosted"]),
				link);

			string html = String.Format("{0} <a href=\"{1}\"><img src=\"{2}\"'></a>",
				by,
				Forum.GetLink( ForumPages.posts,"m={0}#{0}",row["LastMessageID"]),
				GetThemeContents("ICONS","ICON_LATEST")
				);
			return html;
		}

		protected void UnsubscribeTopics_Click(object sender, System.EventArgs e) {
			bool NoneChecked = true;
			for(int i=0;i<TopicList.Items.Count;i++) {
				CheckBox ctrl = (CheckBox)TopicList.Items[i].FindControl("unsubx");
				Label lbl = (Label)TopicList.Items[i].FindControl("ttid");
				if(ctrl.Checked) {
					YAF.Classes.Data.DB.watchtopic_delete(lbl.Text);
					NoneChecked = false;
				}
			}
			if(NoneChecked)
				AddLoadMessage(GetText("WARN_SELECTTOPICS"));
			else
				BindData();
		}

		protected void UnsubscribeForums_Click(object sender, System.EventArgs e) {
			bool NoneChecked = true;
			for(int i=0;i<ForumList.Items.Count;i++) {
				CheckBox ctrl = (CheckBox)ForumList.Items[i].FindControl("unsubf");
				Label lbl = (Label)ForumList.Items[i].FindControl("tfid");
				if(ctrl.Checked) {
					YAF.Classes.Data.DB.watchforum_delete(lbl.Text);
					NoneChecked = false;
				}
			}
			if(NoneChecked)
				AddLoadMessage(GetText("WARN_SELECTFORUMS"));
			else
				BindData();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
