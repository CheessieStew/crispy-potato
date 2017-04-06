using UnityEngine;

class SelectionController
{
    EntityBody _selected;

    public SelectionController() { }

    public EntityBody Selected
    {
        get
        {
            return _selected;
        }
        set
        {
            if (_selected != value)
            {
                if (value != null)
                    value.Selected(true);
                if (_selected != null)
                    _selected.Selected(false);
            }
            _selected = value;
        }
    }

    public void TrySelect()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
            return;

        if (!hit.collider || !hit.collider.transform)
        {
            MonoBehaviour.print("grabbed nothing");
            return;
        }

        MonoBehaviour.print("grabbed " + hit.collider.gameObject);
        Selected = hit.collider.GetComponent<EntityBody>();
    }

    public void Operate()
    {
        if (ControlsManager.Instance != null
            && EntityManager.Instance != null)
        if (Input.GetKeyDown(KeyCode.Mouse0)
            && ControlsManager.Instance.FocusNotOnConsole)
            TrySelect();

        if (Selected != null
            && !Selected.gameObject.activeInHierarchy)
            Selected = null;
    }
}