﻿using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class Column<TableItem> : IColumn<TableItem>
    {
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        private string _title;

        [Parameter]
        public string Title
        {
            get { return _title ?? Property.GetPropertyMemberInfo()?.Name; }
            set { _title = value; }
        }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public bool Sortable { get; set; }

        [Parameter]
        public bool Filterable { get; set; }

        [Parameter]
        public RenderFragment<TableItem> Template { get; set; }

        [Parameter]
        public RenderFragment<TableItem> EditorTemplate { get; set; }

        [Parameter]
        public RenderFragment<Column<TableItem>> CustomIFilters { get; set; }

        [Parameter]
        public Expression<Func<TableItem, object>> Property { get; set; }
        
        [Parameter]
        public Expression<Func<TableItem, bool>> Filter { get; set; }

        public bool SortColumn { get; set; }

        public bool SortDescending { get; set; }

        public bool FilterOpen { get; private set; }

        public Type Type { get; private set; }

        public IFilterManager<TableItem> FilterManager { get; set; }

        public void Dispose()
        {
            this.Table.RemoveColumn(this);
        }

        protected override void OnInitialized()
        {
            Table.AddColumn(this);
        }

        protected override void OnParametersSet()
        {
            if ((Sortable && Property == null) || (Filterable && Property == null))
            {
                throw new InvalidOperationException($"Column {Title} Property parameter is null");
            }

            if (Title == null && Property == null)
            {
                throw new InvalidOperationException("A Column has both Title and Property parameters null");
            }

            Type = Property?.GetPropertyMemberInfo().GetMemberUnderlyingType();
        }

        public void ToggleFilter()
        {
            FilterOpen = !FilterOpen;
            Table.Refresh();
        }

        public void SortBy()
        {
            if (Sortable)
            {
                if (SortColumn)
                {
                    SortDescending = !SortDescending;
                }

                Table.Columns.ForEach(x => x.SortColumn = false);
                
                SortColumn = true;

                Table.Update();
            }
        }
    }
}