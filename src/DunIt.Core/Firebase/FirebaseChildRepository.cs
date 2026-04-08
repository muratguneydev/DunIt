namespace DunIt.Core.Firebase;

using DunIt.Core.Models;
using DunIt.Core.Repositories;

public class FirebaseChildRepository(IFirebaseInterop interop) : IChildRepository
{
    public async Task<Child> AddChild(Child child)
    {
        var dto = new ChildDto(child.Id, child.Name, child.Avatar, child.FirebaseUid.Value);
        var saved = await interop.AddChild(dto);
        return new Child(new ChildId(saved.Id), saved.Name, saved.Avatar, new FirebaseUid(saved.FirebaseUid));
    }

    public Task DeleteChild(ChildId childId) => interop.DeleteChild(childId);

    public async Task<IReadOnlyList<Child>> GetChildren()
    {
        var dtos = await interop.GetChildren();
        return dtos.Select(d => new Child(new ChildId(d.Id), d.Name, d.Avatar, new FirebaseUid(d.FirebaseUid))).ToList();
    }

    public async Task<ISubscription> Subscribe(Action<IReadOnlyList<Child>> onUpdate)
    {
        var id = await interop.SubscribeToChildren(dtos =>
        {
            onUpdate(dtos.Select(d => new Child(new ChildId(d.Id), d.Name, d.Avatar, new FirebaseUid(d.FirebaseUid))).ToList());
            return Task.CompletedTask;
        });
        return new FirebaseSubscription(id, interop);
    }
}
