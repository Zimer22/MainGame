using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
[ExecuteInEditMode]
public class MatrixMaker : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]MatrixMakerHelper matrixMakerHelper;

    [MenuItem("Window/UI Toolkit/MatrixMaker")]
    public static void ShowExample()
    {
        MatrixMaker wnd = GetWindow<MatrixMaker>();
        wnd.titleContent = new GUIContent("MatrixMaker");
    }
    void UpButtonClicked()
    {
        (int,int) value = matrixMakerHelper.MousePos;
        value.Item2--;
        matrixMakerHelper.MousePos = value;
    }
    void DownButtonClicked()
    {
        (int, int) value = matrixMakerHelper.MousePos;
        value.Item2++;  
        matrixMakerHelper.MousePos = value;
    }
    void LeftButtonClicked()
    {
        (int, int) value = matrixMakerHelper.MousePos;
        value.Item1--;
        matrixMakerHelper.MousePos = value;
    }
    void RightButtonClicked()
    {
        (int, int) value = matrixMakerHelper.MousePos;
        value.Item1++;
        matrixMakerHelper.MousePos = value;
    }
    void RefreshClicked()
    {
        matrixMakerHelper.CreateInv();
    }
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Instantiate UXML
        VisualElement UXML = m_VisualTreeAsset.Instantiate();
        root.Add(UXML);
        UnityEngine.UIElements.Button Up = (UnityEngine.UIElements.Button)root.Q("Up");
        UnityEngine.UIElements.Button Down = (UnityEngine.UIElements.Button)root.Q("Down");
        UnityEngine.UIElements.Button Left = (UnityEngine.UIElements.Button)root.Q("Left");
        UnityEngine.UIElements.Button Right = (UnityEngine.UIElements.Button)root.Q("Right");
        UnityEngine.UIElements.Button Refresh = (UnityEngine.UIElements.Button)root.Q("refresh");
        matrixMakerHelper = GameObject.Find("Helper").GetComponent<MatrixMakerHelper>();
        root.dataSource = matrixMakerHelper;
        Up.clicked += UpButtonClicked;
        Down.clicked += DownButtonClicked;
        Left.clicked += LeftButtonClicked;
        Right.clicked += RightButtonClicked;
        Refresh.clicked += RefreshClicked;
    }
}
