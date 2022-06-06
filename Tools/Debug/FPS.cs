using UnityEngine;

public class FPS : PersistentSingleton<FPS>
{
    #region Members

    public enum Anchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    private static string[] stringsFrom00To300 = {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                                  "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                                                  "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                                                  "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                                                  "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
                                                  "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
                                                  "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
                                                  "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
                                                  "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
                                                  "90", "91", "92", "93", "94", "95", "96", "97", "98", "99",
                                                  "100", "101", "102", "103", "104", "105", "106", "107", "108", "109",
                                                  "110", "111", "112", "113", "114", "115", "116", "117", "118", "119",
                                                  "120", "121", "122", "123", "124", "125", "126", "127", "128", "129",
                                                  "130", "131", "132", "133", "134", "135", "136", "137", "138", "139",
                                                  "140", "141", "142", "143", "144", "145", "146", "147", "148", "149",
                                                  "150", "151", "152", "153", "154", "155", "156", "157", "158", "159",
                                                  "160", "161", "162", "163", "164", "165", "166", "167", "168", "169",
                                                  "170", "171", "172", "173", "174", "175", "176", "177", "178", "179",
                                                  "180", "181", "182", "183", "184", "185", "186", "187", "188", "189",
                                                  "190", "191", "192", "193", "194", "195", "196", "197", "198", "199",
                                                  "200", "201", "202", "203", "204", "205", "206", "207", "208", "209",
                                                  "210", "211", "212", "213", "214", "215", "216", "217", "218", "219",
                                                  "220", "221", "222", "223", "224", "225", "226", "227", "228", "229",
                                                  "230", "231", "232", "233", "234", "235", "236", "237", "238", "239",
                                                  "240", "241", "242", "243", "244", "245", "246", "247", "248", "249",
                                                  "250", "251", "252", "253", "254", "255", "256", "257", "258", "259",
                                                  "260", "261", "262", "263", "264", "265", "266", "267", "268", "269",
                                                  "270", "271", "272", "273", "274", "275", "276", "277", "278", "279",
                                                  "280", "281", "282", "283", "284", "285", "286", "287", "288", "289",
                                                  "290", "291", "292", "293", "294", "295", "296", "297", "298", "299",
                                                  "300"};

    [SerializeField]
    private Anchor _anchor;

    [SerializeField]
    private int _fontSize;

    [SerializeField]
    private int _textWidth;

    [SerializeField]
    private Color _textColor;

    private GUIStyle _GUIStyle;
    private string _displayedText;
    private float _framesAccumulated;
    private int _framesDrawnInTheInterval;
    private int _currentFPS;

    #endregion Members

    #region API Methods

    private void Start()
    {
        _framesAccumulated = 0.0f;
        _framesDrawnInTheInterval = 0;
    }

    private void Update()
    {
        _framesDrawnInTheInterval++;
        _framesAccumulated += Time.timeScale / Time.deltaTime;

        if (Time.frameCount % 10 == 0)
        {
            _currentFPS = (int)Mathf.Clamp(_framesAccumulated / _framesDrawnInTheInterval, 0, 300);
            _framesAccumulated = 0.0f;
            _framesDrawnInTheInterval = 0;

            if (_currentFPS >= 0 && _currentFPS <= 300)
                _displayedText = stringsFrom00To300[_currentFPS];
        }
    }

    private void OnGUI()
    {
        if (_GUIStyle == null)
        {
            _GUIStyle = new GUIStyle(GUI.skin.label);
            _GUIStyle.fontSize = _fontSize;
            _GUIStyle.fixedWidth = _textWidth;
            _GUIStyle.normal.textColor = _textColor;

            switch (_anchor)
            {
                case Anchor.TopLeft:
                    _GUIStyle.alignment = TextAnchor.UpperLeft;
                    break;

                case Anchor.TopRight:
                    _GUIStyle.alignment = TextAnchor.UpperRight;
                    break;

                case Anchor.BottomLeft:
                    _GUIStyle.alignment = TextAnchor.LowerLeft;
                    break;

                case Anchor.BottomRight:
                    _GUIStyle.alignment = TextAnchor.LowerRight;
                    break;
            }
        }

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        switch (_anchor)
        {
            case Anchor.TopLeft:
                GUILayout.BeginHorizontal();
                GUILayout.Label(_displayedText, _GUIStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                break;

            case Anchor.TopRight:
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(_displayedText, _GUIStyle);
                GUILayout.EndHorizontal();
                break;

            case Anchor.BottomLeft:
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.Label(_displayedText, _GUIStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                break;

            case Anchor.BottomRight:
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(_displayedText, _GUIStyle);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                break;
        }

        GUILayout.EndArea();
    }

    #endregion API Methods
}