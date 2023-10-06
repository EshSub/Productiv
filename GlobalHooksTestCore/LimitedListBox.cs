using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Productiv
{

    // Create a listbox which will remove the first added item if the item count is greater than a value when adding a new item


    internal class LimitedListBox : ListBox
    {
        public int MaxItems = 10;
        public new LimitedListBoxItemCollection Items;

        public LimitedListBox(): base()
        {
            this.Items = new LimitedListBoxItemCollection(this);
            Items.Add(new ListViewItem("test"));
        }
    }

    internal class LimitedListBoxItemCollection : ListBox.ObjectCollection
    {
        public int MaxItems = 10;
        public LimitedListBoxItemCollection(ListBox owner) : base(owner)
        {
        }
        public void Add(ListViewItem item)
        {
            if (this.Count >= MaxItems)
            {
                this.RemoveAt(0);
            }
            base.Add(item);
        }
    }
}
