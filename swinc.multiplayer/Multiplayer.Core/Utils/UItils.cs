using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Multiplayer.Core
{
	namespace Utils
	{

		namespace Controls
		{
			namespace Window
			{
				/// <summary>
				/// A label for a window
				/// </summary>
				public class UILabel : UIControl
				{
					public Text obj;
					public UILabel(string text, Rect pos, GUIWindow parent, string name = "", bool isBold = false, uint fontsize = 0) : base(pos, parent)
					{
						Text label = WindowManager.SpawnLabel();
						if (!isBold)
							label.text = text;
						else
							label.text = "<b>" + text + "</b>";

						if (fontsize > 0)
							label.fontSize = (int)fontsize;

						if (string.IsNullOrEmpty(name))
							label.name = name;

						Place(label.gameObject);
						obj = label;
					}

					public void ChangeText(string text)
					{
						if (string.IsNullOrEmpty(text))
							obj.text = text;
					}
				}

				/// <summary>
				/// A button for a window
				/// </summary>
				public class UIButton : UIControl
				{
					public Button obj;
					public UIButton(string text, Rect pos, UnityAction action, GUIWindow parent, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
					{
						Button button = WindowManager.SpawnButton();
						button.GetComponentInChildren<Text>().text = text;
						button.onClick.AddListener(action);
						if (string.IsNullOrEmpty(name))
							button.name = name;
						if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
							new UITooltip(tooltiptitle, tooltipdesc, button.gameObject);

						Place(button.gameObject);
						obj = button;
					}

					public void ChangeBehaviour(string text, UnityAction action = null)
					{
						if (string.IsNullOrEmpty(text))
							obj.GetComponentInChildren<Text>().text = text;
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
					public Slider obj;
					public UISlider(int min, int max, int value, Rect pos, GUIWindow parent, UnityAction<float> action = null, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
					{
						Slider slider = WindowManager.SpawnSlider();
						slider.minValue = min;
						slider.maxValue = max;
						slider.value = value;
						if (action != null)
							slider.onValueChanged.AddListener(action);
						if (!string.IsNullOrEmpty(name))
							slider.name = name;
						if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
							new UITooltip(tooltiptitle, tooltipdesc, slider.gameObject);
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
					public GUICombobox obj;
					public UICombobox(string[] items, Rect pos, GUIWindow parent, UnityAction action = null, int selected = 0) : base(pos, parent)
					{
						GUICombobox box = WindowManager.SpawnComboBox();
						box.Items.AddRange(items);
						box.Selected = selected;
						if (action != null)
							box.OnSelectedChanged.AddListener(action);
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
					public Toggle obj;
					public UICheckbox(string label, Rect pos, GUIWindow parent, UnityAction<bool> action = null, bool state = false, string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
					{
						Toggle t = WindowManager.SpawnCheckbox();
						Text ttxt = t.GetComponentInChildren<Text>();
						t.isOn = state;
						ttxt.text = label;
						ttxt.resizeTextForBestFit = true;
						ttxt.alignment = TextAnchor.MiddleCenter;
						if (action != null)
							t.onValueChanged.AddListener(action);
						if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
							new UITooltip(tooltiptitle, tooltipdesc, t.gameObject);
						Place(t.gameObject);
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
							field.onEndEdit.AddListener(endeditaction);
						if (name != "")
							field.name = name;
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
					GUIWindow Parent;
					Rect Pos;
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
					GUIToolTipper obj;
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
							obj.ToolTipValue = title;
						if (!string.IsNullOrEmpty(description))
							obj.TooltipDescription = description;
					}
				}
			}

			namespace Element
			{

				public class UILabel : UIControl
				{
					public Text obj;
					public GameObject gameObject;
					public UILabel(string text, Rect pos, GameObject parent, string name = "", bool isBold = false, uint fontsize = 0, Outline outline = null) : base(pos, parent)
					{
						Text label = WindowManager.SpawnLabel();
						if (!isBold)
							label.text = text;
						else
							label.text = "<b>" + text + "</b>";

						if (fontsize > 0)
							label.fontSize = (int)fontsize;

						if (string.IsNullOrEmpty(name))
							label.name = name;

						Place(label.gameObject);
						obj = label;
						gameObject = label.gameObject;
					}

					public void ChangeText(string text)
					{
						if (string.IsNullOrEmpty(text))
							obj.text = text;
					}
				}

				public class UIButton : UIControl
				{
					public Button obj;
					public UIButton(string text, Rect pos, UnityAction action, GameObject parent, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
					{
						Button button = WindowManager.SpawnButton();
						button.GetComponentInChildren<Text>().text = text;
						button.onClick.AddListener(action);
						if (string.IsNullOrEmpty(name))
							button.name = name;
						if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
							new UITooltip(tooltiptitle, tooltipdesc, button.gameObject);

						Place(button.gameObject);
						obj = button;
					}

					public void ChangeBehaviour(string text, UnityAction action = null)
					{
						if (string.IsNullOrEmpty(text))
							obj.GetComponentInChildren<Text>().text = text;
						if (action != null)
						{
							obj.onClick.RemoveAllListeners();
							obj.onClick.AddListener(action);
						}
					}
				}

				public class UISlider : UIControl
				{
					public Slider obj;
					public UISlider(int min, int max, int value, Rect pos, GameObject parent, UnityAction<float> action = null, string name = "", string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
					{
						Slider slider = WindowManager.SpawnSlider();
						slider.minValue = min;
						slider.maxValue = max;
						slider.value = value;
						if (action != null)
							slider.onValueChanged.AddListener(action);
						if (!string.IsNullOrEmpty(name))
							slider.name = name;
						if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
							new UITooltip(tooltiptitle, tooltipdesc, slider.gameObject);
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
					public GUICombobox obj;
					public UICombobox(string[] items, Rect pos, GameObject parent, UnityAction action = null, int selected = 0) : base(pos, parent)
					{
						GUICombobox box = WindowManager.SpawnComboBox();
						box.Items.AddRange(items);
						box.Selected = selected;
						if (action != null)
							box.OnSelectedChanged.AddListener(action);
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

				public class UIBorder : MaskableGraphic
				{
					[SerializeField]
					float cornerRadius = 5;

					[SerializeField]
					[Range(1, 10)]
					int cornerSegment = 2;

					[SerializeField]
					float width = 10;

					//[SerializeField]
					Texture texture = Texture2D.blackTexture;

					[SerializeField]
					Vector2 tiling = new Vector2(1, 1);

					public override Texture mainTexture
					{
						get
						{
							return texture;
						}
					}

					static List<UIVertex> vertexStream = new List<UIVertex>();
					static List<int> indexStream = new List<int>();

					protected override void OnPopulateMesh(VertexHelper vh)
					{
						var r = GetPixelAdjustedRect();
						vh.Clear();

						GenerateCorner(r, vh);
					}

					void GenerateBox(Rect r, VertexHelper vh)
					{
						float perimeter = r.width * 2 + r.height * 2;

						Color color = this.color;
						if (width > 0)
						{
							//for (int i = 0; i < 4; i++)
							{
								float uv = r.height / perimeter * tiling.x;
								vh.AddVert(new Vector3(r.xMin, r.yMin), color, new Vector2(0, 0));
								vh.AddVert(new Vector3(r.xMin, r.yMax), color, new Vector2(uv, 0));
								vh.AddVert(new Vector3(r.xMin + width, r.yMax - width), color, new Vector2(uv, tiling.y));
								vh.AddVert(new Vector3(r.xMin + width, r.yMin + width), color, new Vector2(0, tiling.y));
								vh.AddTriangle(0, 1, 2);
								vh.AddTriangle(2, 3, 0);
							}

							{
								float uv = (r.height + r.width) / perimeter * tiling.x;
								vh.AddVert(new Vector3(r.xMax, r.yMax), color, new Vector2(uv, 0));
								vh.AddVert(new Vector3(r.xMax - width, r.yMax - width), color, new Vector2(uv, tiling.y));
								vh.AddTriangle(1, 4, 5);
								vh.AddTriangle(5, 2, 1);
							}
							{
								float uv = (r.height + r.width + r.height) / perimeter * tiling.x;
								vh.AddVert(new Vector3(r.xMax, r.yMin), color, new Vector2(uv, 0));
								vh.AddVert(new Vector3(r.xMax - width, r.yMin + width), color, new Vector2(uv, tiling.y));
								vh.AddTriangle(4, 6, 7);
								vh.AddTriangle(7, 5, 4);

							}
							{
								float uv = tiling.x;
								vh.AddVert(new Vector3(r.xMin, r.yMin), color, new Vector2(uv, 0));
								vh.AddVert(new Vector3(r.xMin + width, r.yMin + width), color, new Vector2(uv, tiling.y));
								vh.AddTriangle(6, 8, 9);
								vh.AddTriangle(9, 7, 6);
							}
						}
					}


					void GenerateCorner(Rect rect, VertexHelper vh)
					{
						vertexStream.Clear();
						indexStream.Clear();
						vh.Clear();
						float cornerSize = Mathf.Max(width, cornerRadius);

						float cornerOffset = cornerSize;

						{
							Vector3 begin = new Vector2(rect.xMin, rect.yMin) + new Vector2(0, cornerOffset);
							Vector3 end = new Vector3(rect.xMin, rect.yMax) - new Vector3(0, cornerOffset);
							BeginLine(vertexStream, indexStream, begin, end);
							AddLine(vertexStream, indexStream, begin, end);

							Vector3 cornerBegin = end;
							Vector3 cornerEnd = new Vector3(rect.xMin, rect.yMax) + new Vector3(cornerOffset, 0);
							AddTopLeftCorner(vertexStream, indexStream, cornerBegin, cornerEnd);
						}
						{
							Vector3 begin = new Vector3(rect.xMin, rect.yMax) + new Vector3(cornerOffset, 0);
							Vector3 end = new Vector3(rect.xMax, rect.yMax) - new Vector3(cornerOffset, 0);
							AddLine(vertexStream, indexStream, begin, end);

							Vector3 cornerBegin = end;
							Vector3 cornerEnd = new Vector3(rect.xMax, rect.yMax) - new Vector3(0, cornerOffset, 0);
							AddTopRightCorner(vertexStream, indexStream, cornerBegin, cornerEnd);
						}

						{
							Vector3 begin = new Vector3(rect.xMax, rect.yMax) - new Vector3(0, cornerOffset, 0);
							Vector3 end = new Vector3(rect.xMax, rect.yMin) + new Vector3(0, cornerOffset, 0);
							AddLine(vertexStream, indexStream, begin, end);

							Vector3 cornerBegin = end;
							Vector3 cornerEnd = new Vector3(rect.xMax, rect.yMin) - new Vector3(cornerOffset, 0);
							AddBottomRightCorner(vertexStream, indexStream, cornerBegin, cornerEnd);
						}

						{
							Vector3 begin = new Vector3(rect.xMax, rect.yMin) - new Vector3(cornerOffset, 0);
							Vector3 end = new Vector3(rect.xMin, rect.yMin) + new Vector3(cornerOffset, 0);
							AddLine(vertexStream, indexStream, begin, end);

							Vector3 cornerBegin = end;
							Vector3 cornerEnd = new Vector2(rect.xMin, rect.yMin) + new Vector2(0, cornerOffset);
							AddBottomLeftCorner(vertexStream, indexStream, cornerBegin, cornerEnd);
						}
						EndLine();

						PopulateUV(vertexStream);

						vh.AddUIVertexStream(vertexStream, indexStream);
					}

					void PopulateUV(List<UIVertex> vertexStream)
					{
						float length = 0;
						for (int i = 0; i < vertexStream.Count - 4; i += 2)
						{
							var p0 = vertexStream[i].position;
							var p1 = vertexStream[i + 1].position;
							var p2 = vertexStream[i + 2].position;
							var p3 = vertexStream[i + 3].position;

							length += Vector3.Distance((p0 + p1) * 0.5f, (p2 + p3) * 0.5f);
						}

						float d = 0;

						for (int i = 0; i < vertexStream.Count - 3; i += 2)
						{
							UIVertex v1 = vertexStream[i];
							UIVertex v2 = vertexStream[i + 1];
							UIVertex v3 = vertexStream[i + 2];
							UIVertex v4 = vertexStream[i + 3];

							float y = d / length * tiling.y;
							v1.uv0 = new Vector2(d / length * tiling.y, 0);
							v2.uv0 = new Vector2(y, tiling.x);

							vertexStream[i] = v1;
							vertexStream[i + 1] = v2;

							Vector3 p12 = (v1.position + v2.position) * 0.5f;
							Vector3 p34 = (v3.position + v4.position) * 0.5f;
							d += Vector3.Distance(p12, p34);

							y = d / length * tiling.y;
							v3.uv0 = new Vector2(d / length * tiling.y, 0);
							v4.uv0 = new Vector2(y, tiling.x);

							vertexStream[i + 2] = v3;
							vertexStream[i + 3] = v4;
						}
					}

					/// <summary>
					/// 添加直线
					/// </summary>
					/// <param name="vertexStream"></param>
					/// <param name="indexStream"></param>
					/// <param name="begin"></param>
					/// <param name="end"></param>
					void AddLine(List<UIVertex> vertexStream, List<int> indexStream, Vector3 begin, Vector3 end)
					{
						UIVertex outer = UIVertex.simpleVert;
						UIVertex inner = UIVertex.simpleVert;

						outer.color = color;
						inner.color = color;

						float offset = width;
						Vector3 perpendicular = Vector3.Cross(end - begin, Vector3.forward).normalized;
						outer.position = end;

						inner.position = end + perpendicular * offset;
						vertexStream.Add(outer);
						vertexStream.Add(inner);

						int count = vertexStream.Count;
						{
							indexStream.Add(count - 4);
							indexStream.Add(count - 2);
							indexStream.Add(count - 1);
						}
						{
							indexStream.Add(count - 1);
							indexStream.Add(count - 3);
							indexStream.Add(count - 4);
						}
					}

					void BeginLine(List<UIVertex> vertexStream, List<int> indexStream, Vector3 begin, Vector3 end)
					{
						UIVertex outer = UIVertex.simpleVert;
						UIVertex inner = UIVertex.simpleVert;

						outer.color = color;
						inner.color = color;

						float offset = width;
						Vector3 perpendicular = Vector3.Cross(end - begin, Vector3.forward).normalized;
						outer.position = begin;

						inner.position = begin + perpendicular * offset;

						vertexStream.Add(outer);
						vertexStream.Add(inner);
					}

					void EndLine()
					{

					}

					void AddTopLeftCorner(List<UIVertex> vertexStream, List<int> indexStream, Vector3 begin, Vector3 end)
					{
						/// 使用 innerEdge  半径小于 宽度时，否则 计算 inner Point
						float offset = width - cornerRadius;
						if (offset > 0)
						{
							Vector3 inner = end - new Vector3(0, width);
							AddArc(vertexStream, indexStream, begin + new Vector3(0, offset, 0), end - new Vector3(offset, 0), inner);
							AddSegment(vertexStream, indexStream, end, inner);
						}
						else
						{
							AddArc(vertexStream, indexStream, begin, end);
						}
					}

					void AddTopRightCorner(List<UIVertex> vertexStream, List<int> indexStream, Vector3 begin, Vector3 end)
					{
						/// 使用 innerEdge  半径小于 宽度时，否则 计算 inner Point
						float offset = width - cornerRadius;
						if (offset > 0)
						{
							Vector3 inner = end - new Vector3(width, 0);
							AddArc(vertexStream, indexStream, begin + new Vector3(offset, 0, 0), end + new Vector3(0, offset, 0), inner);
							AddSegment(vertexStream, indexStream, end, inner);
						}
						else
						{
							AddArc(vertexStream, indexStream, begin, end);
						}
					}

					void AddBottomRightCorner(List<UIVertex> vertexStream, List<int> indexStream, Vector3 begin, Vector3 end)
					{
						/// 使用 innerEdge  半径小于 宽度时，否则 计算 inner Point
						float offset = width - cornerRadius;
						if (offset > 0)
						{
							Vector3 inner = end + new Vector3(0, width);
							AddArc(vertexStream, indexStream, begin + new Vector3(0, -offset, 0), end + new Vector3(offset, 0, 0), inner);
							AddSegment(vertexStream, indexStream, end, inner);
						}
						else
						{
							AddArc(vertexStream, indexStream, begin, end);
						}
					}
					void AddBottomLeftCorner(List<UIVertex> vertexStream, List<int> indexStream, Vector3 begin, Vector3 end)
					{
						/// 使用 innerEdge  半径小于 宽度时，否则 计算 inner Point
						float offset = width - cornerRadius;
						if (offset > 0)
						{
							Vector3 inner = end + new Vector3(width, 0);
							AddArc(vertexStream, indexStream, begin + new Vector3(-offset, 0, 0), end + new Vector3(0, -offset, 0), inner);
							AddSegment(vertexStream, indexStream, end, inner);
						}
						else
						{
							AddArc(vertexStream, indexStream, begin, end);
						}
					}

					void AddArc(List<UIVertex> vertexStream, List<int> indexStream, Vector3 arcBegin, Vector3 arcEnd, Vector3? innerEdge = null)
					{
						Vector3 dir = arcEnd - arcBegin;
						Vector3 perpendicular = Vector3.Cross(dir, Vector3.forward).normalized;
						Vector3 center = arcBegin + dir * 0.5f + perpendicular * dir.magnitude * 0.5f;
						//Vector3 innerEdge = center + perpendicular * (width - cornerRadius);

						Vector3 centerDir = arcBegin - center;

						//bool isUseInnerEdge = cornerRadius < width;



						float angle = 90f / cornerSegment;
						for (int i = 0; i <= cornerSegment; i++)
						{
							Quaternion rotation = Quaternion.AngleAxis(-angle * i, Vector3.forward);
							Vector3 newDir = rotation * centerDir;

							Vector3 point1 = center + newDir;
							Vector3 point2 = innerEdge ?? (point1 - newDir.normalized * width);

							AddSegment(vertexStream, indexStream, point1, point2);
						}
					}

					void AddSegment(List<UIVertex> vertexStream, List<int> indexStream, Vector3 outter, Vector3 inner)
					{
						UIVertex v0 = UIVertex.simpleVert;
						v0.position = outter;
						v0.color = color;

						UIVertex v1 = UIVertex.simpleVert;
						v1.position = inner;
						v1.color = color;

						AddSegment(vertexStream, indexStream, v0, v1);
					}

					void AddSegment(List<UIVertex> vertexStream, List<int> indexStream, UIVertex outter, UIVertex inner)
					{
						vertexStream.Add(outter);
						vertexStream.Add(inner);
						int count = vertexStream.Count;
						{
							indexStream.Add(count - 4);
							indexStream.Add(count - 2);
							indexStream.Add(count - 1);
						}
						{
							indexStream.Add(count - 1);
							indexStream.Add(count - 3);
							indexStream.Add(count - 4);
						}
					}

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        var r = GetPixelAdjustedRect();
        width = Mathf.Max(0, Mathf.Min(width, Mathf.Min(r.width, r.height) * 0.5f));
        cornerRadius = Mathf.Max(0, Mathf.Min(cornerRadius, Mathf.Min(r.width, r.height) * 0.5f));

        SetVerticesDirty();
    }
#endif
				}

				public class UICheckbox : UIControl
				{
					public Toggle obj;
					public UICheckbox(string label, Rect pos, GameObject parent, UnityAction<bool> action = null, bool state = false, string tooltiptitle = "", string tooltipdesc = "") : base(pos, parent)
					{
						Toggle t = WindowManager.SpawnCheckbox();
						Text ttxt = t.GetComponentInChildren<Text>();
						t.isOn = state;
						ttxt.text = label;
						ttxt.resizeTextForBestFit = true;
						ttxt.alignment = TextAnchor.MiddleCenter;
						if (action != null)
							t.onValueChanged.AddListener(action);
						if (!string.IsNullOrEmpty(tooltiptitle) && !string.IsNullOrEmpty(tooltipdesc))
							new UITooltip(tooltiptitle, tooltipdesc, t.gameObject);
						Place(t.gameObject);
					}
				}

				public class UITextbox : UIControl
				{
					public InputField obj;
					public UITextbox(Rect pos, GameObject parent, string placeholder = "", string name = "", UnityAction<string> endeditaction = null, int fontsize = 15, bool isPassword = false) : base(pos, parent)
					{
						InputField field = WindowManager.SpawnInputbox();
						if (endeditaction != null)
							field.onEndEdit.AddListener(endeditaction);
						if (name != "")
							field.name = name;
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
					GameObject Parent;
					Rect Pos;
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
					GUIToolTipper obj;
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
							obj.ToolTipValue = title;
						if (!string.IsNullOrEmpty(description))
							obj.TooltipDescription = description;
					}
				}
			}
		}
	}
}
