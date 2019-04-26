#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-24 오후 3:34:58
 *	개요 : 
   ============================================ */
#endregion Header

// 이걸 쓰면 TypeFilter가 동작을 안한다.. 보류
#if !ODIN_INSPECTOR

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.Utilities;
using Sirenix.OdinInspector;

//
// TODO: Rewrite ListDrawer completely!
// Make a utility for drawing lists that dictioanries, hashsets, etc can utialize.
// And use the new DragAndDropUtilities instead of the old and broken DragAndDropmanager.
// Handle both drag and drop event, in the same method. Preferably, instead of having order dependency which is impossible
// with cross window dragging etc...
//

public static class CollectionDrawerStaticInfo
{
    public static InspectorProperty CurrentDraggingPropertyInfo;
    public static InspectorProperty CurrentDroppingPropertyInfo;
    public static DelayedGUIDrawer DelayedGUIDrawer = new DelayedGUIDrawer();
    internal static Action NextCustomAddFunction;
}

/// <summary>
/// Property drawer for anything that has a <see cref="ICollectionResolver"/>.
/// </summary>
[AllowGUIEnabledForReadonly]
[DrawerPriority(0, 0, 0)]
public class CollectionDrawer_Extension<T> : OdinValueDrawer<T>, IDefinesGenericMenuItems
{
    private static GUILayoutOption[] listItemOptions = GUILayoutOptions.MinHeight(25).ExpandWidth(true);
    private ListDrawerConfigInfo info;
    private string errorMessage;

    protected override bool CanDrawValueProperty(InspectorProperty property)
    {
        return property.ChildResolver is ICollectionResolver;
    }

    void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
    {
        if (property.ValueEntry.WeakSmartValue == null)
        {
            return;
        }

        var resolver = property.ChildResolver as ICollectionResolver;

        bool isReadOnly = resolver.IsReadOnly;

        var config = property.GetAttribute<ListDrawerSettingsAttribute>();
        bool isEditable = isReadOnly == false && property.ValueEntry.IsEditable && (config == null || (!config.IsReadOnlyHasValue) || (config.IsReadOnlyHasValue && config.IsReadOnly == false));
        bool pasteElement = isEditable && Clipboard.CanPaste(resolver.ElementType);
        bool clearList = isEditable && property.Children.Count > 0;

        //if (genericMenu.GetItemCount() > 0 && (pasteElement || clearList))
        //{
        //    genericMenu.AddSeparator(null);
        //}

        if (pasteElement)
        {
            genericMenu.AddItem(new GUIContent("Paste Element"), false, () =>
            {
                (property.ChildResolver as ICollectionResolver).QueueAdd(new object[] { Clipboard.Paste() });
                GUIHelper.RequestRepaint();
            });
        }

        if (clearList)
        {
            genericMenu.AddItem(new GUIContent("Clear Collection"), false, () =>
            {
                (property.ChildResolver as ICollectionResolver).QueueClear();
                GUIHelper.RequestRepaint();
            });
        }
        else
        {
            genericMenu.AddDisabledItem(new GUIContent("Clear Collection"));
        }
    }

    /// <summary>
    /// Initializes the drawer.
    /// </summary>
    protected override void Initialize()
    {
        var resolver = this.Property.ChildResolver as ICollectionResolver;
        bool isReadOnly = resolver.IsReadOnly;

        var customListDrawerOptions = this.Property.GetAttribute<ListDrawerSettingsAttribute>() ?? new ListDrawerSettingsAttribute();
        isReadOnly = this.ValueEntry.IsEditable == false || isReadOnly || customListDrawerOptions.IsReadOnlyHasValue && customListDrawerOptions.IsReadOnly;

        info = new ListDrawerConfigInfo()
        {
            StartIndex = 0,
            Toggled = this.ValueEntry.Context.GetPersistent<bool>(this, "ListDrawerToggled", customListDrawerOptions.ExpandedHasValue ? customListDrawerOptions.Expanded : GeneralDrawerConfig.Instance.OpenListsByDefault),
            RemoveAt = -1,

            // Now set further down, so it can be kept updated every frame
            //Label = new GUIContent(label == null || string.IsNullOrEmpty(label.text) ? this.Property.ValueEntry.TypeOfValue.GetNiceName() : label.text, label == null ? string.Empty : label.tooltip),
            ShowAllWhilePaging = false,
            EndIndex = 0,
            CustomListDrawerOptions = customListDrawerOptions,
            IsReadOnly = isReadOnly,
            Draggable = !isReadOnly && (!customListDrawerOptions.IsReadOnlyHasValue),
            HideAddButton = isReadOnly || customListDrawerOptions.HideAddButton,
            HideRemoveButton = isReadOnly || customListDrawerOptions.HideRemoveButton,
        };

        info.ListConfig = GeneralDrawerConfig.Instance;
        info.Property = this.Property;

        if (customListDrawerOptions.DraggableHasValue && !customListDrawerOptions.DraggableItems)
        {
            info.Draggable = false;
        }

        if (!(this.Property.ChildResolver is IOrderedCollectionResolver))
        {
            info.Draggable = false;
        }

        if (info.CustomListDrawerOptions.OnBeginListElementGUI != null)
        {
            string error;
            MemberInfo memberInfo = this.Property.ParentType
                .FindMember()
                .IsMethod()
                .IsNamed(info.CustomListDrawerOptions.OnBeginListElementGUI)
                .HasParameters<int>()
                .ReturnsVoid()
                .GetMember<MethodInfo>(out error);

            if (memberInfo == null || error != null)
            {
                // TOOD: Do something about this "There should really be an error message here." thing.
                // For this to trigger both the member and the error message should be null. Can that happen?
                this.errorMessage = error ?? "There should really be an error message here.";
            }
            else
            {
                info.OnBeginListElementGUI = EmitUtilities.CreateWeakInstanceMethodCaller<int>(memberInfo as MethodInfo);
            }
        }

        if (info.CustomListDrawerOptions.OnEndListElementGUI != null)
        {
            string error;
            MemberInfo memberInfo = this.Property.ParentType
                .FindMember()
                .IsMethod()
                .IsNamed(info.CustomListDrawerOptions.OnEndListElementGUI)
                .HasParameters<int>()
                .ReturnsVoid()
                .GetMember<MethodInfo>(out error);

            if (memberInfo == null || error != null)
            {
                // TOOD: Do something about this "There should really be an error message here." thing.
                // For this to trigger both the member and the error message should be null. Can that happen?
                this.errorMessage = error ?? "There should really be an error message here.";
            }
            else
            {
                info.OnEndListElementGUI = EmitUtilities.CreateWeakInstanceMethodCaller<int>(memberInfo as MethodInfo);
            }
        }

        if (info.CustomListDrawerOptions.OnTitleBarGUI != null)
        {
            string error;
            MemberInfo memberInfo = this.Property.ParentType
                .FindMember()
                .IsMethod()
                .IsNamed(info.CustomListDrawerOptions.OnTitleBarGUI)
                .HasNoParameters()
                .ReturnsVoid()
                .GetMember<MethodInfo>(out error);

            if (memberInfo == null || error != null)
            {
                // TOOD: Do something about this "There should really be an error message here." thing.
                // For this to trigger both the member and the error message should be null. Can that happen?
                this.errorMessage = error ?? "There should really be an error message here.";
            }
            else
            {
                info.OnTitleBarGUI = EmitUtilities.CreateWeakInstanceMethodCaller(memberInfo as MethodInfo);
            }
        }

        if (info.CustomListDrawerOptions.ListElementLabelName != null)
        {
            string error;
            MemberInfo memberInfo = resolver.ElementType
                .FindMember()
                .HasNoParameters()
                .IsNamed(info.CustomListDrawerOptions.ListElementLabelName)
                .HasReturnType<object>(true)
                .GetMember(out error);

            if (memberInfo == null || error != null)
            {
                // TOOD: Do something about this "There should really be an error message here." thing.
                // For this to trigger both the member and the error message should be null. Can that happen?
                this.errorMessage = error ?? "There should really be an error message here.";
            }
            else
            {
                string methodSuffix = memberInfo as MethodInfo == null ? "" : "()";
                info.GetListElementLabelText = DeepReflection.CreateWeakInstanceValueGetter(resolver.ElementType, typeof(object), info.CustomListDrawerOptions.ListElementLabelName + methodSuffix);
            }
        }

        // Resolve custom add method member reference.
        if (info.CustomListDrawerOptions.CustomAddFunction != null)
        {
            string error;
            MemberInfo memberInfo = this.Property.ParentType
                .FindMember()
                .HasNoParameters()
                .IsNamed(info.CustomListDrawerOptions.CustomAddFunction)
                .IsInstance()
                .HasReturnType(resolver.ElementType)
                .GetMember(out error);

            if (memberInfo == null || error != null)
            {
                string error2;

                memberInfo = this.Property.ParentType
                   .FindMember()
                   .IsMethod()
                   .HasNoParameters()
                   .IsNamed(info.CustomListDrawerOptions.CustomAddFunction)
                   .IsInstance()
                   .ReturnsVoid()
                   .GetMember(out error2);

                if (memberInfo == null || error2 != null)
                {
                    this.errorMessage = error + " - or - " + error2;
                }
                else
                {
                    info.GetCustomAddFunctionVoid = EmitUtilities.CreateWeakInstanceMethodCaller(memberInfo as MethodInfo);
                }
            }
            else
            {
                string methodSuffix = memberInfo as MethodInfo == null ? "" : "()";
                info.GetCustomAddFunction = DeepReflection.CreateWeakInstanceValueGetter(
                    this.Property.ParentType,
                    resolver.ElementType,
                    info.CustomListDrawerOptions.CustomAddFunction + methodSuffix
                );
            }
        }

        // Resolve custom remove index method member reference.
        if (info.CustomListDrawerOptions.CustomRemoveIndexFunction != null)
        {
            if (this.Property.ChildResolver is IOrderedCollectionResolver == false)
            {
                this.errorMessage = "ListDrawerSettings.CustomRemoveIndexFunction is invalid on unordered collections. Use ListDrawerSetings.CustomRemoveElementFunction instead.";
            }
            else
            {
                MethodInfo method = this.Property.ParentType
                    .FindMember()
                    .IsNamed(info.CustomListDrawerOptions.CustomRemoveIndexFunction)
                    .IsMethod()
                    .IsInstance()
                    .HasParameters<int>()
                    .ReturnsVoid()
                    .GetMember<MethodInfo>(out this.errorMessage);

                if (method != null)
                {
                    info.CustomRemoveIndexFunction = EmitUtilities.CreateWeakInstanceMethodCaller<int>(method);
                }
            }
        }
        // Resolve custom remove element method member reference.
        else if (info.CustomListDrawerOptions.CustomRemoveElementFunction != null)
        {
            var element = (this.Property.ChildResolver as ICollectionResolver).ElementType;

            MethodInfo method = this.Property.ParentType
                .FindMember()
                .IsNamed(info.CustomListDrawerOptions.CustomRemoveElementFunction)
                .IsMethod()
                .IsInstance()
                .HasParameters(element)
                .ReturnsVoid()
                .GetMember<MethodInfo>(out this.errorMessage);

            if (method != null)
            {
                // TOOD: Emit dis.
                info.CustomRemoveElementFunction = (o, e) => method.Invoke(o, new object[] { e });
            }
        }
    }

    /// <summary>
    /// Draws the property.
    /// </summary>
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var resolver = this.Property.ChildResolver as ICollectionResolver;
        bool isReadOnly = resolver.IsReadOnly;

        if (this.errorMessage != null)
        {
            SirenixEditorGUI.ErrorMessageBox(errorMessage);
        }

        if (info.Label == null || (label != null && label.text != info.Label.text))
        {
            info.Label = new GUIContent(label == null || string.IsNullOrEmpty(label.text) ? Property.ValueEntry.TypeOfValue.GetNiceName() : label.text, label == null ? string.Empty : label.tooltip);
        }

        info.IsReadOnly = resolver.IsReadOnly;

        info.ListItemStyle.padding.left = info.Draggable ? 25 : 7;
        info.ListItemStyle.padding.right = info.IsReadOnly || info.HideRemoveButton ? 4 : 20;

        if (Event.current.type == EventType.Repaint)
        {
            info.DropZoneTopLeft = GUIUtility.GUIToScreenPoint(new Vector2(0, 0));
        }

        info.CollectionResolver = Property.ChildResolver as ICollectionResolver;
        info.OrderedCollectionResolver = Property.ChildResolver as IOrderedCollectionResolver;
        info.Count = Property.Children.Count;
        info.IsEmpty = Property.Children.Count == 0;

        SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
        this.BeginDropZone();
        {
            this.DrawToolbar();
            if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(Property, this), info.Toggled.Value))
            {
                GUIHelper.PushLabelWidth(GUIHelper.BetterLabelWidth - info.ListItemStyle.padding.left);
                this.DrawItems();
                GUIHelper.PopLabelWidth();
            }
            SirenixEditorGUI.EndFadeGroup();
        }
        this.EndDropZone();
        SirenixEditorGUI.EndIndentedVertical();

        if (info.OrderedCollectionResolver != null)
        {
            if (info.RemoveAt >= 0 && Event.current.type == EventType.Repaint)
            {
                try
                {
                    if (info.CustomRemoveIndexFunction != null)
                    {
                        foreach (var parent in this.Property.ParentValues)
                        {
                            info.CustomRemoveIndexFunction(
                                parent,
                                info.RemoveAt);
                        }
                    }
                    else if (info.CustomRemoveElementFunction != null)
                    {
                        for (int i = 0; i < this.Property.ParentValues.Count; i++)
                        {
                            info.CustomRemoveElementFunction(
                                this.Property.ParentValues[i],
                                this.Property.Children[info.RemoveAt].ValueEntry.WeakValues[i]);
                        }
                    }
                    else
                    {
                        info.OrderedCollectionResolver.QueueRemoveAt(info.RemoveAt);
                    }
                }
                finally
                {

                    info.RemoveAt = -1;
                }

                GUIHelper.RequestRepaint();
            }
        }
        else if (info.RemoveValues != null && Event.current.type == EventType.Repaint)
        {
            try
            {
                if (info.CustomRemoveElementFunction != null)
                {
                    for (int i = 0; i < this.Property.ParentValues.Count; i++)
                    {
                        info.CustomRemoveElementFunction(
                            this.Property.ParentValues[i],
                            this.info.RemoveValues[i]);
                    }
                }
                else
                {
                    info.CollectionResolver.QueueRemove(info.RemoveValues);
                }
            }
            finally
            {

                info.RemoveValues = null;
            }
            GUIHelper.RequestRepaint();
        }

        if (info.ObjectPicker != null && info.ObjectPicker.IsReadyToClaim && Event.current.type == EventType.Repaint)
        {
            var value = info.ObjectPicker.ClaimObject();

            if (info.JumpToNextPageOnAdd)
            {
                info.StartIndex = int.MaxValue;
            }

            object[] values = new object[info.Property.Tree.WeakTargets.Count];

            values[0] = value;
            for (int j = 1; j < values.Length; j++)
            {
                values[j] = SerializationUtility.CreateCopy(value);
            }

            info.CollectionResolver.QueueAdd(values);
        }
    }

    private DropZoneHandle BeginDropZone()
    {
        if (info.OrderedCollectionResolver == null) return null;

        var dropZone = DragAndDropManager.BeginDropZone(info.Property.Tree.GetHashCode() + "-" + info.Property.Path, info.CollectionResolver.ElementType, true);

        if (Event.current.type == EventType.Repaint && DragAndDropManager.IsDragInProgress)
        {
            var rect = dropZone.Rect;
            dropZone.Rect = rect;
        }

        dropZone.Enabled = info.IsReadOnly == false;
        info.DropZone = dropZone;
        return dropZone;
    }

    private static UnityEngine.Object[] HandleUnityObjectsDrop(ListDrawerConfigInfo info)
    {
        if (info.IsReadOnly) return null;

        var eventType = Event.current.type;
        if (eventType == EventType.Layout)
        {
            info.IsAboutToDroppingUnityObjects = false;
        }
        if ((eventType == EventType.DragUpdated || eventType == EventType.DragPerform) && info.DropZone.Rect.Contains(Event.current.mousePosition))
        {
            UnityEngine.Object[] objReferences = null;

            if (DragAndDrop.objectReferences.Any(n => n != null && info.CollectionResolver.ElementType.IsAssignableFrom(n.GetType())))
            {
                objReferences = DragAndDrop.objectReferences.Where(x => x != null && info.CollectionResolver.ElementType.IsAssignableFrom(x.GetType())).Reverse().ToArray();
            }
            else if (info.CollectionResolver.ElementType.InheritsFrom(typeof(Component)))
            {
                objReferences = DragAndDrop.objectReferences.OfType<GameObject>().Select(x => x.GetComponent(info.CollectionResolver.ElementType)).Where(x => x != null).Reverse().ToArray();
            }
            else if (info.CollectionResolver.ElementType.InheritsFrom(typeof(Sprite)) && DragAndDrop.objectReferences.Any(n => n is Texture2D && AssetDatabase.Contains(n)))
            {
                objReferences = DragAndDrop.objectReferences.OfType<Texture2D>().Select(x =>
                {
                    var path = AssetDatabase.GetAssetPath(x);
                    return AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }).Where(x => x != null).Reverse().ToArray();
            }

            bool acceptsDrag = objReferences != null && objReferences.Length > 0;

            if (acceptsDrag)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
                info.IsAboutToDroppingUnityObjects = true;
                info.IsDroppingUnityObjects = info.IsAboutToDroppingUnityObjects;
                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    return objReferences;
                }
            }
        }
        if (eventType == EventType.Repaint)
        {
            info.IsDroppingUnityObjects = info.IsAboutToDroppingUnityObjects;
        }
        return null;
    }

    private void EndDropZone()
    {
        if (info.OrderedCollectionResolver == null) return;

        if (info.DropZone.IsReadyToClaim)
        {
            CollectionDrawerStaticInfo.CurrentDraggingPropertyInfo = null;
            CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo = info.Property;
            object droppedObject = info.DropZone.ClaimObject();

            object[] values = new object[info.Property.Tree.WeakTargets.Count];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = droppedObject;
            }

            if (info.DropZone.IsCrossWindowDrag)
            {
                // If it's a cross-window drag, the changes will for some reason be lost if we don't do this.
                GUIHelper.RequestRepaint();
                EditorApplication.delayCall += () =>
                {
                    info.OrderedCollectionResolver.QueueInsertAt(Mathf.Clamp(info.InsertAt, 0, info.Property.Children.Count), values);
                };
            }
            else
            {
                info.OrderedCollectionResolver.QueueInsertAt(Mathf.Clamp(info.InsertAt, 0, info.Property.Children.Count), values);
            }
        }
        else
        {
            UnityEngine.Object[] droppedObjects = HandleUnityObjectsDrop(info);
            if (droppedObjects != null)
            {
                foreach (var obj in droppedObjects)
                {
                    object[] values = new object[info.Property.Tree.WeakTargets.Count];

                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = obj;
                    }

                    info.OrderedCollectionResolver.QueueInsertAt(Mathf.Clamp(info.InsertAt, 0, info.Property.Children.Count), values);
                }
            }
        }
        DragAndDropManager.EndDropZone();
    }

    private void DrawToolbar()
    {
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            // Label
            if (info.DropZone != null && DragAndDropManager.IsDragInProgress && info.DropZone.IsAccepted == false)
            {
                GUIHelper.PushGUIEnabled(false);
            }

            if (info.Property.ValueEntry.ListLengthChangedFromPrefab)
            {
                GUIHelper.PushIsBoldLabel(true);
            }

            if (info.ListConfig.HideFoldoutWhileEmpty && info.IsEmpty || info.CustomListDrawerOptions.Expanded)
            {
                GUILayout.Label(info.Label, GUILayoutOptions.ExpandWidth(false));
            }
            else
            {
                info.Toggled.Value = SirenixEditorGUI.Foldout(info.Toggled.Value, info.Label ?? GUIContent.none);
            }

            if (info.Property.ValueEntry.ListLengthChangedFromPrefab)
            {
                GUIHelper.PopIsBoldLabel();
            }

            if (info.CustomListDrawerOptions.Expanded)
            {
                info.Toggled.Value = true;
            }

            if (info.DropZone != null && DragAndDropManager.IsDragInProgress && info.DropZone.IsAccepted == false)
            {
                GUIHelper.PopGUIEnabled();
            }

            GUILayout.FlexibleSpace();

            // Item Count
            if (info.CustomListDrawerOptions.ShowItemCountHasValue ? info.CustomListDrawerOptions.ShowItemCount : info.ListConfig.ShowItemCount)
            {
                if (info.Property.ValueEntry.ValueState == PropertyValueState.CollectionLengthConflict)
                {
                    GUILayout.Label(info.Count + " / " + info.CollectionResolver.MaxCollectionLength + " items", EditorStyles.centeredGreyMiniLabel);
                }
                else
                {
                    GUILayout.Label(info.IsEmpty ? "Empty" : info.Count + " items", EditorStyles.centeredGreyMiniLabel);
                }
            }

            bool paging = info.CustomListDrawerOptions.PagingHasValue ? info.CustomListDrawerOptions.ShowPaging : true;
            bool hidePaging =
                    info.ListConfig.HidePagingWhileCollapsed && info.Toggled.Value == false ||
                    info.ListConfig.HidePagingWhileOnlyOnePage && info.Count <= info.NumberOfItemsPerPage;

            int numberOfItemsPrPage = Math.Max(1, info.NumberOfItemsPerPage);
            int numberOfPages = Mathf.CeilToInt(info.Count / (float)numberOfItemsPrPage);
            int pageIndex = info.Count == 0 ? 0 : (info.StartIndex / numberOfItemsPrPage) % info.Count;

            // Paging
            if (paging)
            {
                bool disablePaging = paging && !hidePaging && (DragAndDropManager.IsDragInProgress || info.ShowAllWhilePaging || info.Toggled.Value == false);
                if (disablePaging)
                {
                    GUIHelper.PushGUIEnabled(false);
                }

                if (!hidePaging)
                {
                    if (pageIndex == 0) { GUIHelper.PushGUIEnabled(false); }

                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.TriangleLeft, true))
                    {
                        if (Event.current.button == 0)
                        {
                            info.StartIndex -= numberOfItemsPrPage;
                        }
                        else
                        {
                            info.StartIndex = 0;
                        }
                    }
                    if (pageIndex == 0) { GUIHelper.PopGUIEnabled(); }

                    var userPageIndex = EditorGUILayout.IntField((numberOfPages == 0 ? 0 : (pageIndex + 1)), GUILayoutOptions.Width(10 + numberOfPages.ToString(CultureInfo.InvariantCulture).Length * 10)) - 1;
                    if (pageIndex != userPageIndex)
                    {
                        info.StartIndex = userPageIndex * numberOfItemsPrPage;
                    }

                    GUILayout.Label("/ " + numberOfPages);

                    if (pageIndex == numberOfPages - 1) { GUIHelper.PushGUIEnabled(false); }

                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.TriangleRight, true))
                    {
                        if (Event.current.button == 0)
                        {
                            info.StartIndex += numberOfItemsPrPage;
                        }
                        else
                        {
                            info.StartIndex = numberOfItemsPrPage * numberOfPages;
                        }
                    }
                    if (pageIndex == numberOfPages - 1) { GUIHelper.PopGUIEnabled(); }
                }

                pageIndex = info.Count == 0 ? 0 : (info.StartIndex / numberOfItemsPrPage) % info.Count;

                var newStartIndex = Mathf.Clamp(pageIndex * numberOfItemsPrPage, 0, Mathf.Max(0, info.Count - 1));
                if (newStartIndex != info.StartIndex)
                {
                    info.StartIndex = newStartIndex;
                    var newPageIndex = info.Count == 0 ? 0 : (info.StartIndex / numberOfItemsPrPage) % info.Count;
                    if (pageIndex != newPageIndex)
                    {
                        pageIndex = newPageIndex;
                        info.StartIndex = Mathf.Clamp(pageIndex * numberOfItemsPrPage, 0, Mathf.Max(0, info.Count - 1));
                    }
                }

                info.EndIndex = Mathf.Min(info.StartIndex + numberOfItemsPrPage, info.Count);

                if (disablePaging)
                {
                    GUIHelper.PopGUIEnabled();
                }
            }
            else
            {
                info.StartIndex = 0;
                info.EndIndex = info.Count;
            }

            if (paging && hidePaging == false && info.ListConfig.ShowExpandButton)
            {
                if (info.Count < 300)
                {
                    if (SirenixEditorGUI.ToolbarButton(info.ShowAllWhilePaging ? EditorIcons.TriangleUp : EditorIcons.TriangleDown, true))
                    {
                        info.ShowAllWhilePaging = !info.ShowAllWhilePaging;
                    }
                }
                else
                {
                    info.ShowAllWhilePaging = false;
                }
            }

            // Add Button
            if (info.IsReadOnly == false && !info.HideAddButton)
            {
                info.ObjectPicker = ObjectPicker.GetObjectPicker(info, info.CollectionResolver.ElementType);
                var superHackyAddFunctionWeSeriouslyNeedANewListDrawer = CollectionDrawerStaticInfo.NextCustomAddFunction;
                CollectionDrawerStaticInfo.NextCustomAddFunction = null;

                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
                {
                    if (superHackyAddFunctionWeSeriouslyNeedANewListDrawer != null)
                    {
                        superHackyAddFunctionWeSeriouslyNeedANewListDrawer();
                    }
                    else if (info.GetCustomAddFunction != null)
                    {
                        var objs = new object[info.Property.Tree.WeakTargets.Count];

                        for (int i = 0; i < objs.Length; i++)
                        {
                            objs[i] = info.GetCustomAddFunction(info.Property.ParentValues[i]);
                        }

                        info.CollectionResolver.QueueAdd(objs);
                    }
                    else if (info.GetCustomAddFunctionVoid != null)
                    {
                        info.GetCustomAddFunctionVoid(info.Property.ParentValues[0]);

                        this.Property.ValueEntry.WeakValues.ForceMarkDirty();
                    }
                    else if (info.CustomListDrawerOptions.AlwaysAddDefaultValue)
                    {
                        var objs = new object[info.Property.Tree.WeakTargets.Count];

                        if (info.Property.ValueEntry.SerializationBackend == SerializationBackend.Unity)
                        {
                            for (int i = 0; i < objs.Length; i++)
                            {
                                objs[i] = UnitySerializationUtility.CreateDefaultUnityInitializedObject(info.CollectionResolver.ElementType);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < objs.Length; i++)
                            {
                                if (info.CollectionResolver.ElementType.IsValueType)
                                {
                                    objs[i] = Activator.CreateInstance(info.CollectionResolver.ElementType);
                                }
                                else
                                {
                                    objs[i] = null;
                                }
                            }
                        }

                        //info.ListValueChanger.AddListElement(objs, "Add default value");
                        info.CollectionResolver.QueueAdd(objs);
                    }
                    else if (info.CollectionResolver.ElementType.InheritsFrom<UnityEngine.Object>() && Event.current.modifiers == EventModifiers.Control)
                    {
                        info.CollectionResolver.QueueAdd(new object[info.Property.Tree.WeakTargets.Count]);
                    }
                    else
                    {
                        info.ObjectPicker.ShowObjectPicker(
                            null,
                            info.Property.GetAttribute<AssetsOnlyAttribute>() == null,
                            GUIHelper.GetCurrentLayoutRect(),
                            info.Property.ValueEntry.SerializationBackend == SerializationBackend.Unity);
                    }
                }

                info.JumpToNextPageOnAdd = paging && (info.Count % numberOfItemsPrPage == 0) && (pageIndex + 1 == numberOfPages);
            }

            if (info.OnTitleBarGUI != null)
            {
                info.OnTitleBarGUI(info.Property.ParentValues[0]);
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    virtual protected void DrawItems()
    {
        int from = 0;
        int to = info.Count;
        bool paging = info.CustomListDrawerOptions.PagingHasValue ? info.CustomListDrawerOptions.ShowPaging : true;
        if (paging && info.ShowAllWhilePaging == false)
        {
            from = Mathf.Clamp(info.StartIndex, 0, info.Count);
            to = Mathf.Clamp(info.EndIndex, 0, info.Count);
        }

        var drawEmptySpace = info.DropZone != null && info.DropZone.IsBeingHovered || info.IsDroppingUnityObjects;
        float height = drawEmptySpace ? info.IsDroppingUnityObjects ? 16 : (DragAndDropManager.CurrentDraggingHandle.Rect.height - 3) : 0;
        var rect = SirenixEditorGUI.BeginVerticalList();
        {
            for (int i = 0, j = from, k = from; j < to; i++, j++)
            {
                var dragHandle = this.BeginDragHandle(j, i);
                {
                    if (drawEmptySpace)
                    {
                        var topHalf = dragHandle.Rect;
                        topHalf.height /= 2;
                        if (topHalf.Contains(info.LayoutMousePosition) || topHalf.y > info.LayoutMousePosition.y && i == 0)
                        {
                            GUILayout.Space(height);
                            drawEmptySpace = false;
                            info.InsertAt = k;
                        }
                    }

                    if (dragHandle.IsDragging == false)
                    {
                        k++;
                        this.DrawItem(info.Property.Children[j], dragHandle, j);
                    }
                    else
                    {
                        GUILayout.Space(3);
                        CollectionDrawerStaticInfo.DelayedGUIDrawer.Begin(dragHandle.Rect.width, dragHandle.Rect.height, dragHandle.CurrentMethod != DragAndDropMethods.Move);
                        DragAndDropManager.AllowDrop = false;
                        this.DrawItem(info.Property.Children[j], dragHandle, j);
                        DragAndDropManager.AllowDrop = true;
                        CollectionDrawerStaticInfo.DelayedGUIDrawer.End();
                        if (dragHandle.CurrentMethod != DragAndDropMethods.Move)
                        {
                            GUILayout.Space(3);
                        }
                    }

                    if (drawEmptySpace)
                    {
                        var bottomHalf = dragHandle.Rect;
                        bottomHalf.height /= 2;
                        bottomHalf.y += bottomHalf.height;

                        if (bottomHalf.Contains(info.LayoutMousePosition) || bottomHalf.yMax < info.LayoutMousePosition.y && j + 1 == to)
                        {
                            GUILayout.Space(height);
                            drawEmptySpace = false;
                            info.InsertAt = Mathf.Min(k, to);
                        }
                    }
                }
                this.EndDragHandle(i);
            }

            if (drawEmptySpace)
            {
                GUILayout.Space(height);
                info.InsertAt = Event.current.mousePosition.y > rect.center.y ? to : from;
            }

            if (to == info.Property.Children.Count && info.Property.ValueEntry.ValueState == PropertyValueState.CollectionLengthConflict)
            {
                SirenixEditorGUI.BeginListItem(false);
                GUILayout.Label(GUIHelper.TempContent("------"), EditorStyles.centeredGreyMiniLabel);
                SirenixEditorGUI.EndListItem();
            }
        }
        SirenixEditorGUI.EndVerticalList();

        if (Event.current.type == EventType.Repaint)
        {
            info.LayoutMousePosition = Event.current.mousePosition;
        }
    }

    private void EndDragHandle(int i)
    {
        var handle = DragAndDropManager.EndDragHandle();

        if (handle.IsDragging)
        {
            info.Property.Tree.DelayAction(() =>
            {
                if (DragAndDropManager.CurrentDraggingHandle != null)
                {
                    CollectionDrawerStaticInfo.DelayedGUIDrawer.Draw(Event.current.mousePosition - DragAndDropManager.CurrentDraggingHandle.MouseDownPostionOffset);
                }
            });
        }
    }

    private DragHandle BeginDragHandle(int j, int i)
    {
        var child = info.Property.Children[j];
        var dragHandle = DragAndDropManager.BeginDragHandle(child, child.ValueEntry.WeakSmartValue, info.IsReadOnly ? DragAndDropMethods.Reference : DragAndDropMethods.Move);
        dragHandle.Enabled = info.Draggable;

        if (dragHandle.OnDragStarted)
        {
            CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo = null;
            CollectionDrawerStaticInfo.CurrentDraggingPropertyInfo = info.Property.Children[j];
            dragHandle.OnDragFinnished = dropEvent =>
            {
                if (dropEvent == DropEvents.Moved)
                {
                    if (dragHandle.IsCrossWindowDrag || (CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo != null && CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo.Tree != info.Property.Tree))
                    {
                            // Make sure drop happens a bit later, as deserialization and other things sometimes
                            // can override the change.
                            GUIHelper.RequestRepaint();
                        EditorApplication.delayCall += () =>
                        {
                            info.OrderedCollectionResolver.QueueRemoveAt(j);
                        };
                    }
                    else
                    {
                        info.OrderedCollectionResolver.QueueRemoveAt(j);
                    }
                }

                CollectionDrawerStaticInfo.CurrentDraggingPropertyInfo = null;
            };
        }

        return dragHandle;
    }

    virtual protected Rect DrawItem(InspectorProperty itemProperty, DragHandle dragHandle, int index = -1)
    {
        var listItemInfo = itemProperty.Context.Get<ListItemInfo>(this, "listItemInfo");

        Rect rect;
        rect = SirenixEditorGUI.BeginListItem(false, info.ListItemStyle, listItemOptions);
        {
            if (Event.current.type == EventType.Repaint && !info.IsReadOnly)
            {
                listItemInfo.Value.Width = rect.width;
                dragHandle.DragHandleRect = new Rect(rect.x + 4, rect.y, 20, rect.height);
                listItemInfo.Value.DragHandleRect = new Rect(rect.x + 4, rect.y + 2 + ((int)rect.height - 23) / 2, 20, 20);
                listItemInfo.Value.RemoveBtnRect = new Rect(listItemInfo.Value.DragHandleRect.x + rect.width - 22, listItemInfo.Value.DragHandleRect.y + 1, 14, 14);

                if (info.HideRemoveButton == false)
                {

                }
                if (info.Draggable)
                {
                    GUI.Label(listItemInfo.Value.DragHandleRect, EditorIcons.List.Inactive, GUIStyle.none);
                }
            }

            GUIHelper.PushHierarchyMode(false);
            GUIContent label = null;

            if (info.CustomListDrawerOptions.ShowIndexLabelsHasValue)
            {
                if (info.CustomListDrawerOptions.ShowIndexLabels)
                {
                    label = new GUIContent(index.ToString());
                }
            }
            else if (info.ListConfig.ShowIndexLabels)
            {
                label = new GUIContent(index.ToString());
            }

            if (info.GetListElementLabelText != null)
            {
                var value = itemProperty.ValueEntry.WeakSmartValue;

                if (object.ReferenceEquals(value, null))
                {
                    if (label == null)
                    {
                        label = new GUIContent("Null");
                    }
                    else
                    {
                        label.text += " : Null";
                    }
                }
                else
                {
                    label = label ?? new GUIContent("");
                    if (label.text != "") label.text += " : ";

                    object text = info.GetListElementLabelText(value);
                    label.text += (text == null ? "" : text.ToString());
                }
            }

            if (info.OnBeginListElementGUI != null)
            {
                info.OnBeginListElementGUI(info.Property.ParentValues[0], index);
            }
            itemProperty.Draw(label);

            if (info.OnEndListElementGUI != null)
            {
                info.OnEndListElementGUI(info.Property.ParentValues[0], index);
            }

            GUIHelper.PopHierarchyMode();

            if (info.IsReadOnly == false && info.HideRemoveButton == false)
            {
                if (SirenixEditorGUI.IconButton(listItemInfo.Value.RemoveBtnRect, EditorIcons.X))
                {
                    if (info.OrderedCollectionResolver != null)
                    {
                        if (index >= 0)
                        {
                            info.RemoveAt = index;
                        }
                    }
                    else
                    {
                        var values = new object[itemProperty.ValueEntry.ValueCount];

                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = itemProperty.ValueEntry.WeakValues[i];
                        }

                        info.RemoveValues = values;
                    }
                }
            }
        }
        SirenixEditorGUI.EndListItem();

        return rect;
    }

    protected struct ListItemInfo
    {
        public float Width;
        public Rect RemoveBtnRect;
        public Rect DragHandleRect;
    }

    protected class ListDrawerConfigInfo
    {
        public ICollectionResolver CollectionResolver;
        public IOrderedCollectionResolver OrderedCollectionResolver;
        public bool IsEmpty;
        public ListDrawerSettingsAttribute CustomListDrawerOptions;
        public int Count;
        public LocalPersistentContext<bool> Toggled;
        public int StartIndex;
        public int EndIndex;
        public DropZoneHandle DropZone;
        public Vector2 LayoutMousePosition;
        public Vector2 DropZoneTopLeft;
        public int InsertAt;
        public int RemoveAt;
        public object[] RemoveValues;
        public bool IsReadOnly;
        public bool Draggable;
        public bool ShowAllWhilePaging;
        public ObjectPicker ObjectPicker;
        public bool JumpToNextPageOnAdd;
        public Action<object> OnTitleBarGUI;
        public GeneralDrawerConfig ListConfig;
        public InspectorProperty Property;
        public GUIContent Label;
        public bool IsAboutToDroppingUnityObjects;
        public bool IsDroppingUnityObjects;
        public bool HideAddButton;
        public bool HideRemoveButton;

        public Action<object> GetCustomAddFunctionVoid;
        public Func<object, object> GetCustomAddFunction;

        public Action<object, int> CustomRemoveIndexFunction;
        public Action<object, object> CustomRemoveElementFunction;

        public Func<object, object> GetListElementLabelText;
        public Action<object, int> OnBeginListElementGUI;
        public Action<object, int> OnEndListElementGUI;

        public int NumberOfItemsPerPage
        {
            get
            {
                return this.CustomListDrawerOptions.NumberOfItemsPerPageHasValue ? this.CustomListDrawerOptions.NumberOfItemsPerPage : this.ListConfig.NumberOfItemsPrPage;
            }
        }

        public GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(25, 20, 3, 3)
        };
    }
}
#endif