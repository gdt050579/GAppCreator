using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GAppCreator
{
    public class PreviewControl: UserControl
    {
        protected Object SelectedObject;
        protected Project prj;
        protected ImageList SmallImageList;
        public void SetPreviewObject(Project proj, ImageList smallImageList,Object obj)
        {
            SelectedObject = obj;
            prj = proj;
            SmallImageList = smallImageList;
            OnNewPreviewObject();
        }
        public virtual void OnNewPreviewObject()
        {
        }
    }
}
