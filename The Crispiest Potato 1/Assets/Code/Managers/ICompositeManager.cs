namespace Assets.Code.Managers
{
    interface ICompositeManager
    {
        bool ChildrenManagersInitiated { get; }
        void InitiateChildrenManagers();
        void OperateChildrenManagers();
    }
}