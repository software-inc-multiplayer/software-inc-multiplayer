using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    namespace Utils
    {
#pragma warning disable IDE0044 // Add readonly modifier
        namespace Controls
        {
            namespace Window
            {
                /// <summary>
                /// A label for a window
                /// </summary>
                public class UILabel : UIControl
                {

                    private Text obj;

                    public UILabel(string text, Rect pos, GUIWindow parent, string name = "", bool isBold = false, uint fontsize = 0) : base(pos, parent)
                    {
                        Text label = WindowManager.SpawnLabel();
                        if (!isBold)
                        {
                            label.text = text;
                        }
                        else
                        {
                            label.text = "<b>" + text + "</b>";
                        }

                        if (fontsize > 0)
                        {
                            label.fontSize = (int)fontsize;
                        }

                        if (string.IsNullOrEmpty(name))
                        {
                            label.name = name;
                        }

                        Place(label.gameObject);
                        obj = label;
                    }

                    public void ChangeText(string text)
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            obj.text = text;
                        }
                    }
                }

                /// <summary>
                /// A button for a window
                /// </summary>
                public class UIButton : UIControl
                {
                    private Button obj;
                    public UIButton(string text, Rect pos, UnityAction action, GUIWindow parent, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
                    {
                        Button button = WindowManager.SpawnButton();
                        button.GetComponentInChildren<Text>().text = text;
                        button.onClick.AddListener(action);
                        if (string.IsNullOrEmpty(name))
                        {
                            button.name = name;
                        }

                        if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
                        {
                            new UITooltip(tooltiptitle, tooltipdesc, button.gameObject);
                        }

                        Place(button.gameObject);
                        obj = button;
                    }

                    public void ChangeBehaviour(string text, UnityAction action = null)
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            obj.GetComponentInChildren<Text>().text = text;
                        }

                        if (action != null)
                        {
                            obj.onClick.RemoveAllListeners();
                            obj.onClick.AddListener(action);
                        }
                    }
                }

                /// <summary>
                /// A slider for a window
                /// </summary>
                public class UISlider : UIControl
                {
                    private Slider obj;
                    public UISlider(int min, int max, int value, Rect pos, GUIWindow parent, UnityAction<float> action = null, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
                    {
                        Slider slider = WindowManager.SpawnSlider();
                        slider.minValue = min;
                        slider.maxValue = max;
                        slider.value = value;
                        if (action != null)
                        {
                            slider.onValueChanged.AddListener(action);
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            slider.name = name;
                        }

                        if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
                        {
                            new UITooltip(tooltiptitle, tooltipdesc, slider.gameObject);
                        }

                        Place(slider.gameObject);
                        obj = slider;
                    }

                    public void Change(int min, int max, int value)
                    {
                        obj.minValue = min;
                        obj.maxValue = max;
                        obj.value = value;
                    }
                }

                /// <summary>
                /// A combobox for a window
                /// </summary>
                public class UICombobox : UIControl
                {
                    private GUICombobox obj;
                    public UICombobox(string[] items, Rect pos, GUIWindow parent, UnityAction action = null, int selected = 0) : base(pos, parent)
                    {
                        GUICombobox box = WindowManager.SpawnComboBox();
                        box.Items.AddRange(items);
                        box.Selected = selected;
                        if (action != null)
                        {
                            box.OnSelectedChanged.AddListener(action);
                        }

                        Place(box.gameObject);
                        obj = box;
                    }

                    public void ChangeContent(string[] items, int selected = 0)
                    {
                        obj.Items.Clear();
                        obj.Items.AddRange(items);
                        obj.Selected = selected;
                    }
                }

                /// <summary>
                /// a checkbox for a window
                /// </summary>
                public class UICheckbox : UIControl
                {
                    private Toggle obj;
                    public UICheckbox(string label, Rect pos, GUIWindow parent, UnityAction<bool> action = null, bool state = false, string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
                    {
                        obj = WindowManager.SpawnCheckbox();
                        Text ttxt = obj.GetComponentInChildren<Text>();
                        obj.isOn = state;
                        ttxt.text = label;
                        ttxt.resizeTextForBestFit = true;
                        ttxt.alignment = TextAnchor.MiddleCenter;
                        if (action != null)
                        {
                            obj.onValueChanged.AddListener(action);
                        }

                        if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
                        {
                            new UITooltip(tooltiptitle, tooltipdesc, obj.gameObject);
                        }

                        Place(obj.gameObject);
                    }
                }

                /// <summary>
                /// a textbox for a window
                /// </summary>
                public class UITextbox : UIControl
                {
                    public InputField obj;
                    public UITextbox(Rect pos, GUIWindow parent, string placeholder = "", string name = "", UnityAction<string> endeditaction = null, int fontsize = 15) : base(pos, parent)
                    {
                        InputField field = WindowManager.SpawnInputbox();
                        if (endeditaction != null)
                        {
                            field.onEndEdit.AddListener(endeditaction);
                        }

                        if (name != "")
                        {
                            field.name = name;
                        }

                        var texts = field.GetComponentsInChildren<Text>();
                        field.text = "";
                        texts[0].resizeTextForBestFit = true;
                        texts[0].alignment = TextAnchor.MiddleCenter;
                        texts[0].text = placeholder;
                        texts[1].fontSize = fontsize;
                        texts[1].alignment = TextAnchor.MiddleCenter;
                        Place(field.gameObject);
                        obj = field;
                    }
                }

                /// <summary>
                /// The main control handler, do not use this directly but use the controls based on it!
                /// </summary>
                public class UIControl
                {
                    private GUIWindow Parent;
                    private Rect Pos;
                    public UIControl(Rect pos, GUIWindow parent)
                    {
                        Pos = pos;
                        Parent = parent;
                    }

                    public void Place(GameObject go)
                    {
                        WindowManager.AddElementToWindow(go, Parent, Pos, new Rect(0, 0, 0, 0));
                    }
                }

                /// <summary>
                /// A tooltip for the controls, will be added if you set tooltiptitle & tooltipdesc within the controls but can added manually as well
                /// </summary>
                public class UITooltip
                {
                    private GUIToolTipper obj;
                    public UITooltip(string title, string description, GameObject go)
                    {
                        GUIToolTipper tt = (GUIToolTipper)go.AddComponent(typeof(GUIToolTipper));
                        tt.Localize = false;
                        tt.TooltipDescription = description;
                        tt.ToolTipValue = title;
                        obj = tt;
                    }

                    public void Change(string title, string description)
                    {
                        if (!string.IsNullOrEmpty(title))
                        {
                            obj.ToolTipValue = title;
                        }

                        if (!string.IsNullOrEmpty(description))
                        {
                            obj.TooltipDescription = description;
                        }
                    }
                }
            }

            namespace Element
            {

                public class UILabel : UIControl
                {
                    public Text obj;
                    public GameObject gameObject;
                    public UILabel(string text, Rect pos, GameObject parent, string name = "", bool isBold = false, uint fontsize = 0) : base(pos, parent)
                    {
                        Text label = WindowManager.SpawnLabel();
                        if (!isBold)
                        {
                            label.text = text;
                        }
                        else
                        {
                            label.text = "<b>" + text + "</b>";
                        }

                        if (fontsize > 0)
                        {
                            label.fontSize = (int)fontsize;
                        }

                        if (string.IsNullOrEmpty(name))
                        {
                            label.name = name;
                        }

                        Place(label.gameObject);
                        obj = label;
                        gameObject = label.gameObject;
                    }

                    public void ChangeText(string text)
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            obj.text = text;
                        }
                    }
                }

                public class UIButton : UIControl
                {
                    private Button obj;
                    public UIButton(string text, Rect pos, UnityAction action, GameObject parent, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
                    {
                        Button button = WindowManager.SpawnButton();
                        button.GetComponentInChildren<Text>().text = text;
                        button.onClick.AddListener(action);
                        if (string.IsNullOrEmpty(name))
                        {
                            button.name = name;
                        }

                        if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
                        {
                            new UITooltip(tooltiptitle, tooltipdesc, button.gameObject);
                        }

                        Place(button.gameObject);
                        obj = button;
                    }

                    public void ChangeBehaviour(string text, UnityAction action = null)
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            obj.GetComponentInChildren<Text>().text = text;
                        }

                        if (action != null)
                        {
                            obj.onClick.RemoveAllListeners();
                            obj.onClick.AddListener(action);
                        }
                    }
                }

                public class UISlider : UIControl
                {
                    private Slider obj;
                    public UISlider(int min, int max, int value, Rect pos, GameObject parent, UnityAction<float> action = null, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
                    {
                        Slider slider = WindowManager.SpawnSlider();
                        slider.minValue = min;
                        slider.maxValue = max;
                        slider.value = value;
                        if (action != null)
                        {
                            slider.onValueChanged.AddListener(action);
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            slider.name = name;
                        }

                        if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
                        {
                            new UITooltip(tooltiptitle, tooltipdesc, slider.gameObject);
                        }

                        Place(slider.gameObject);
                        obj = slider;
                    }

                    public void Change(int min, int max, int value)
                    {
                        obj.minValue = min;
                        obj.maxValue = max;
                        obj.value = value;
                    }
                }

                public class UICombobox : UIControl
                {
                    private GUICombobox obj;
                    public UICombobox(string[] items, Rect pos, GameObject parent, UnityAction action = null, int selected = 0) : base(pos, parent)
                    {
                        GUICombobox box = WindowManager.SpawnComboBox();
                        box.Items.AddRange(items);
                        box.Selected = selected;
                        if (action != null)
                        {
                            box.OnSelectedChanged.AddListener(action);
                        }

                        Place(box.gameObject);
                        obj = box;
                    }

                    public void ChangeContent(string[] items, int selected = 0)
                    {
                        obj.Items.Clear();
                        obj.Items.AddRange(items);
                        obj.Selected = selected;
                    }
                }
                public class UICheckbox : UIControl
                {
                    private Toggle obj;
                    public UICheckbox(string label, Rect pos, GameObject parent, UnityAction<bool> action = null, bool state = false, string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
                    {
                        obj = WindowManager.SpawnCheckbox();
                        Text ttxt = obj.GetComponentInChildren<Text>();
                        obj.isOn = state;
                        ttxt.text = label;
                        ttxt.resizeTextForBestFit = true;
                        ttxt.alignment = TextAnchor.MiddleCenter;
                        if (action != null)
                        {
                            obj.onValueChanged.AddListener(action);
                        }

                        if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
                        {
                            new UITooltip(tooltiptitle, tooltipdesc, obj.gameObject);
                        }

                        Place(obj.gameObject);
                    }
                }

                public class UITextbox : UIControl
                {
                    public InputField obj;
                    public UITextbox(Rect pos, GameObject parent, string placeholder = "", string name = "", UnityAction<string> endeditaction = null, int fontsize = 15, bool isPassword = false) : base(pos, parent)
                    {
                        InputField field = WindowManager.SpawnInputbox();
                        if (endeditaction != null)
                        {
                            field.onEndEdit.AddListener(endeditaction);
                        }

                        if (name != "")
                        {
                            field.name = name;
                        }

                        var texts = field.GetComponentsInChildren<Text>();
                        field.text = "";
                        if (isPassword)
                        {
                            field.contentType = InputField.ContentType.Password;
                        }
                        texts[0].resizeTextForBestFit = true;
                        texts[0].alignment = TextAnchor.MiddleCenter;
                        texts[0].text = placeholder;
                        texts[1].fontSize = fontsize;
                        texts[1].alignment = TextAnchor.MiddleCenter;
                        Place(field.gameObject);
                        obj = field;
                    }
                }

                public class UIControl
                {
                    private GameObject Parent;
                    private Rect Pos;
                    public UIControl(Rect pos, GameObject parent)
                    {
                        Pos = pos;
                        Parent = parent;
                    }

                    public void Place(GameObject go)
                    {
                        WindowManager.AddElementToElement(go, Parent, Pos, new Rect(0, 0, 0, 0));
                    }
                }

                public class UITooltip
                {
                    private GUIToolTipper obj;
                    public UITooltip(string title, string description, GameObject go)
                    {
                        GUIToolTipper tt = (GUIToolTipper)go.AddComponent(typeof(GUIToolTipper));
                        tt.Localize = false;
                        tt.TooltipDescription = description;
                        tt.ToolTipValue = title;
                        obj = tt;
                    }

                    public void Change(string title, string description)
                    {
                        if (!string.IsNullOrEmpty(title))
                        {
                            obj.ToolTipValue = title;
                        }

                        if (!string.IsNullOrEmpty(description))
                        {
                            obj.TooltipDescription = description;
                        }
                    }
                }
            }
        }
#pragma warning restore IDE0044 // Add readonly modifier
    }
}
