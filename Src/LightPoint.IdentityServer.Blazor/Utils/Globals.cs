using AntDesign;
using LightPoint.IdentityServer.Blazor.Utils.Modules.Tools;

namespace LightPoint.IdentityServer.Blazor.Utils
{
    public class Globals
    {
        public Globals(Modals modals, ModalService modalService,
            IMessageService messageService)
        {
            modals.ModalService = modalService;
            Modals = modals;
            MessageService = messageService;
        }
        public Modals Modals { get; }
        public IMessageService MessageService { get; }
    }
}
