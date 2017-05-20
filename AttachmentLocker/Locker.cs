using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

    class Locker
    {
    private Client owner;
    private List<WeaponComponent> storedAttachments;


    public Locker(Client owner)
    {
        this.owner = owner;
        this.storedAttachments = new List<WeaponComponent>();
    }

   
    public Client getOwner()
    {
        return owner;
    }

    public List<WeaponComponent> getStoredAttachments()
    {
        return storedAttachments;
    }

    public void removeAttachment(Client owner, WeaponComponent removedAttachment)
    {
        storedAttachments.Remove(removedAttachment);
    }

    public void addAttachment(Client owner, WeaponComponent addedAttachment)
    {
        storedAttachments.Add(addedAttachment);
    }

}
