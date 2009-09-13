using System;
using OpenMetaverse;
using System.Windows.Forms;

namespace Radegast
{
    public class AvatarProfileAction : ContextAction
    {
        public AvatarProfileAction(RadegastInstance inst)
            : base(inst)
        {
            Label = "Profile";
            ContextType = typeof(Avatar);
        }
        public override bool IsEnabled(object target)
        {
            return true;
        }
        public override void OnInvoke(object sender, EventArgs e, object target)
        {
            UUID id = ToUUID(target);
            if (id == UUID.Zero) id = ToUUID(sender);
            if (id == UUID.Zero)
            {
                DebugLog("cannot find avatar" + target);
                return;
            }
            (new frmProfile(instance, instance.getAvatarName(id), id)).Show();
        }
    }
}