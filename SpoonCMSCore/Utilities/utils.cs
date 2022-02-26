using System;
using System.Collections.Generic;
using System.Text;
using DiffMatchPatch;
using SpoonCMSCore.Classes;

namespace SpoonCMSCore.Utilities
{
    public static class Utils
    {
        static string MinifyJS(string jsCode)
        {
            string minifiedCode = "";


            return minifiedCode;
        }

        private static string GetValueDiff(string newVal, string oldVal)
        {
            var dmp = DiffMatchPatch.DiffMatchPatchModule.Default;
            List<Diff> diffs = dmp.DiffMain(oldVal, newVal);
            string html = dmp.DiffPrettyHtml(diffs);

            return html;
        }

        /// <summary>
        /// Update a content item with new values and add old value to the history
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ContentItem UpdateContentItem(ContentItem sourceItem, ContentItem newItem)
        {
            if (sourceItem.Value != newItem.Value)
            {
                ContentValueHistory newHistory = new ContentValueHistory();
                newHistory.Changed = DateTime.Now;
                newHistory.Value = sourceItem.Value;
                newHistory.ValueDiffHTML = Utils.GetValueDiff(sourceItem.Value, newItem.Value);

                if (sourceItem.History == null)
                {
                    sourceItem.History = new LimitedList<ContentValueHistory>(5);
                }

                sourceItem.History.Add(newHistory);
            }

            sourceItem.Active = newItem.Active;
            sourceItem.BeginDate = newItem.BeginDate;
            sourceItem.EndDate = newItem.EndDate;
            sourceItem.Value = newItem.Value;

            //Why was I not doing this before? Did I miss it or is there a reason?
            sourceItem.Priority = newItem.Priority;            

            return sourceItem;
        }

        public static Container UpdateContainer(Container sourceItem, Container newItem)
        {
            sourceItem.Name = newItem.Name;
            sourceItem.Active = newItem.Active;
            //sourceItem.Items = newItem.Items;

            //loop through and remove items that aren't in anymore
            foreach(string key in sourceItem.Items.Keys)
            {
                if(!newItem.Items.ContainsKey(key) || newItem.Items[key].Id != sourceItem.Items[key].Id)
                {
                    sourceItem.Items.Remove(key);
                }
            }

            //Shouldn't have to worry about dupe keys anymore
            //Loop through and add or update content items that match
            //simplier to add a holder list but stuff could get big and too much mem
            foreach(string key in newItem.Items.Keys)
            {
                if(sourceItem.Items.ContainsKey(key))
                {
                    UpdateContentItem(sourceItem.Items[key], newItem.Items[key]);
                }
                else
                {
                    sourceItem.Items.Add(key, newItem.Items[key]);
                }
            }

            return sourceItem;
        }
    }
}
