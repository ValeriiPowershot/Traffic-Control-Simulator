using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace TrafficControl.Elections
{
    [DisallowMultipleComponent]
    public sealed class ElectionDemo : MonoBehaviour
    {
        [Header("Presentation")]
        public Font RegularFont, BoldFont;
        [Range(0.5f, 10)] public float CountDuration = 3.5f;
        [SerializeField] private Canvas dashboard;
        [SerializeField] private Text mayorNumber, opponentNumber, undecidedNumber, shareNumber, phaseLabel;
        [SerializeField] private Text mayorPercent, opponentPercent, undecidedCaption, resultHeading, resultBody;
        [SerializeField] private Text turnoutLabel, countLabel, verdictBadge, actionLabel, noteLabel;
        [SerializeField] private Image mayorBar, opponentBar, countFill, verdictStripe;
        [SerializeField] private Button mayorPlus, mayorMinus, opponentPlus, opponentMinus, primary, reset;
        [SerializeField] private Button winPreset, losePreset, tiePreset, emptyPreset;
        private ElectionSession session;
        private float countElapsed;
        private bool bound;
        public ElectionSession Session => session;
        public float CountProgress => session != null && session.Phase != ElectionPhase.Preparation ?
            session.Phase == ElectionPhase.Results ? 1 : Mathf.Clamp01(countElapsed / Mathf.Max(0.1f, CountDuration)) : 0;
        private static readonly Color Ink = Hex(0x163A35), Muted = Hex(0x71817A), Green = Hex(0x25765A), Gold = Hex(0xD79E45);
        private static readonly CultureInfo Russian = CultureInfo.GetCultureInfo("ru-RU");

        private void Awake()
        {
            session = new ElectionSession();
            if (!dashboard) BuildInterface();
            EnsureInput(); BindButtons(); Refresh();
        }
        public void ChangeMayor(int amount) { if (session.AdjustMayor(amount)) Refresh(); }
        public void ChangeOpponent(int amount) { if (session.AdjustOpponent(amount)) Refresh(); }
        public void SetExample(int mayor, int opponent) { if (session.SetVotes(mayor, opponent)) Refresh(); }
        public void BeginElection()
        { if (session.BeginCount()) { countElapsed = 0; Refresh(); } }
        public void FinishElection()
        { if (session.CompleteCount()) { countElapsed = CountDuration; Refresh(); } }
        public void ResetElection() { session.Reset(); countElapsed = 0; Refresh(); }
        private void Update()
        {
            ReadKeyboard();
            if (session.Phase == ElectionPhase.Counting)
            {
                countElapsed += Time.unscaledDeltaTime;
                if (countElapsed >= CountDuration) FinishElection(); else Refresh();
            }
        }
        private void ReadKeyboard()
        {
            bool up, down, left, right, enter, space, restart, shift, one, two, three, zero;
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            up = keyboard.upArrowKey.wasPressedThisFrame; down = keyboard.downArrowKey.wasPressedThisFrame;
            left = keyboard.leftArrowKey.wasPressedThisFrame; right = keyboard.rightArrowKey.wasPressedThisFrame;
            enter = keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame;
            space = keyboard.spaceKey.wasPressedThisFrame; restart = keyboard.rKey.wasPressedThisFrame;
            shift = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
            one = keyboard.digit1Key.wasPressedThisFrame; two = keyboard.digit2Key.wasPressedThisFrame;
            three = keyboard.digit3Key.wasPressedThisFrame; zero = keyboard.digit0Key.wasPressedThisFrame;
#else
            up = UnityEngine.Input.GetKeyDown(KeyCode.UpArrow); down = UnityEngine.Input.GetKeyDown(KeyCode.DownArrow);
            left = UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow); right = UnityEngine.Input.GetKeyDown(KeyCode.RightArrow);
            enter = UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter);
            space = UnityEngine.Input.GetKeyDown(KeyCode.Space); restart = UnityEngine.Input.GetKeyDown(KeyCode.R);
            shift = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
            one = UnityEngine.Input.GetKeyDown(KeyCode.Alpha1); two = UnityEngine.Input.GetKeyDown(KeyCode.Alpha2);
            three = UnityEngine.Input.GetKeyDown(KeyCode.Alpha3); zero = UnityEngine.Input.GetKeyDown(KeyCode.Alpha0);
#endif
            if (restart) { ResetElection(); return; }
            if (session.Phase == ElectionPhase.Counting) { if (space || enter) FinishElection(); return; }
            if (session.Phase == ElectionPhase.Results) return;
            int step = shift ? 500 : 100;
            if (up) ChangeMayor(step); if (down) ChangeMayor(-step);
            if (right) ChangeOpponent(step); if (left) ChangeOpponent(-step);
            if (one) SetExample(6000, 3000); if (two) SetExample(3200, 5800);
            if (three) SetExample(4500, 4500); if (zero) SetExample(0, 0);
            if (enter) BeginElection();
        }
        private void EnsureInput()
        {
            if (!FindObjectOfType<EventSystem>())
            {
                var go = new GameObject("Election input", typeof(EventSystem)); go.transform.SetParent(transform, false);
                go.GetComponent<EventSystem>().sendNavigationEvents = false;
#if ENABLE_INPUT_SYSTEM
                go.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
#else
                go.AddComponent<StandaloneInputModule>();
#endif
            }
        }
        private void BindButtons()
        {
            if (bound) return; bound = true;
            mayorPlus.onClick.AddListener(() => ChangeMayor(100)); mayorMinus.onClick.AddListener(() => ChangeMayor(-100));
            opponentPlus.onClick.AddListener(() => ChangeOpponent(100)); opponentMinus.onClick.AddListener(() => ChangeOpponent(-100));
            primary.onClick.AddListener(() => { if (session.Phase == ElectionPhase.Preparation) BeginElection(); else if (session.Phase == ElectionPhase.Counting) FinishElection(); else ResetElection(); });
            reset.onClick.AddListener(ResetElection);
            winPreset.onClick.AddListener(() => SetExample(6000, 3000)); losePreset.onClick.AddListener(() => SetExample(3200, 5800));
            tiePreset.onClick.AddListener(() => SetExample(4500, 4500)); emptyPreset.onClick.AddListener(() => SetExample(0, 0));
        }
        private static string Number(int number) => number.ToString("N0", Russian).Replace('\u00a0', ' ').Replace('\u202f', ' ');
        private static string Percent(double fraction) => (fraction * 100).ToString("F1", Russian) + "%";
        public void Refresh()
        {
            if (session == null || !dashboard) return;
            bool preparing = session.Phase == ElectionPhase.Preparation, counting = session.Phase == ElectionPhase.Counting;
            var result = session.Result;
            float progress = CountProgress;
            int mayor = counting ? Mathf.FloorToInt(result.MayorVotes * progress) : session.MayorVotes;
            int opponent = counting ? Mathf.FloorToInt(result.OpponentVotes * progress) : session.OpponentVotes;
            int total = session.MayorVotes + session.OpponentVotes;
            double share = total == 0 ? 0 : (double)session.MayorVotes / total;
            mayorNumber.text = Number(mayor); opponentNumber.text = Number(opponent); undecidedNumber.text = Number(session.Unassigned);
            shareNumber.text = counting ? "···" : Percent(share);
            mayorPercent.text = Percent((double)mayor / session.Electorate) + " избирателей";
            opponentPercent.text = Percent((double)opponent / session.Electorate) + " избирателей";
            undecidedCaption.text = preparing ? "Ещё не назначенные голоса" : "Не участвовали в голосовании";
            SetBar(mayorBar, (float)mayor / session.Electorate, 900);
            SetBar(opponentBar, (float)opponent / session.Electorate, 900);
            SetBar(countFill, progress, 490);
            turnoutLabel.text = "Явка  " + Percent((double)total / session.Electorate) + "    /    Избирателей  " + Number(session.Electorate);
            countLabel.text = preparing ? "ГОТОВНОСТЬ К ГОЛОСОВАНИЮ" : counting ? "ПОДСЧИТАНО  " + Mathf.FloorToInt(progress * 100) + "%" : "ПОДСЧЁТ ЗАВЕРШЁН";
            phaseLabel.text = preparing ? "01 / ПОДГОТОВКА" : counting ? "02 / ПОДСЧЁТ ГОЛОСОВ" : "03 / ИТОГИ ВЫБОРОВ";
            foreach (var button in new[] {mayorPlus, mayorMinus, opponentPlus, opponentMinus, winPreset, losePreset, tiePreset, emptyPreset}) button.interactable = preparing;
            mayorPlus.interactable = preparing && session.Unassigned > 0; opponentPlus.interactable = preparing && session.Unassigned > 0;
            mayorMinus.interactable = preparing && session.MayorVotes > 0; opponentMinus.interactable = preparing && session.OpponentVotes > 0;
            actionLabel.text = preparing ? "Провести выборы     Enter" : counting ? "Показать результат     Space" : "Новые выборы     R";
            Color accent = Green;
            if (preparing)
            {
                resultHeading.text = "Город решает";
                resultBody.text = "Распределите голоса между кандидатами.\nЗатем проведите итоговое голосование.";
                verdictBadge.text = "ИТОГ ЕЩЁ НЕ ОПРЕДЕЛЁН";
                noteLabel.text = "Для победы нужно больше половины поданных голосов.\nНичья означает повторное голосование.";
            }
            else if (counting)
            {
                resultHeading.text = "Считаем голоса…";
                resultBody.text = "Избирательные участки передают результаты.\nРаспределение голосов зафиксировано.";
                verdictBadge.text = "РАБОТАЕТ ИЗБИРКОМ";
                noteLabel.text = "Space или Enter — завершить подсчёт сразу.\nR — начать проверку заново.";
            }
            else
            {
                switch (result.Outcome)
                {
                    case ElectionOutcome.MayorWon:
                        resultHeading.text = "Вы переизбраны"; verdictBadge.text = "НОВЫЙ СРОК ПОЛНОМОЧИЙ";
                        resultBody.text = "Жители доверили вам следующий срок.\nПреимущество: " + Number(result.MayorVotes - result.OpponentVotes) + " голосов."; break;
                    case ElectionOutcome.OpponentWon:
                        accent = Hex(0xA85443); resultHeading.text = "Победил соперник"; verdictBadge.text = "СМЕНА ГОРОДСКОЙ ВЛАСТИ";
                        resultBody.text = "В этот раз жители выбрали другого кандидата.\nРазница: " + Number(result.OpponentVotes - result.MayorVotes) + " голосов."; break;
                    case ElectionOutcome.Tie:
                        accent = Gold; resultHeading.text = "Голоса разделились"; verdictBadge.text = "ПОВТОРНОЕ ГОЛОСОВАНИЕ";
                        resultBody.text = "У кандидатов одинаковое число голосов.\nДля определения победителя нужны новые выборы."; break;
                    default:
                        accent = Muted; resultHeading.text = "Выборы не состоялись"; verdictBadge.text = "НЕТ ПОДАННЫХ ГОЛОСОВ";
                        resultBody.text = "Ни за одного кандидата не подано голосов.\nНазначьте голоса и проведите выборы снова."; break;
                }
                noteLabel.text = "За мэра: " + Percent(result.MayorShare) + " поданных голосов.\nРезультат этой проверки не сохраняется в кампанию.";
            }
            verdictStripe.color = accent; verdictBadge.color = accent;
        }

        // Builds ordinary, editable uGUI objects. The baked scene keeps this hierarchy.
        [ContextMenu("Build election interface")]
        public void BuildInterface()
        {
            if (dashboard) return;
            if (!RegularFont) RegularFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (!BoldFont) BoldFont = RegularFont;
            var canvasObject = new GameObject("Election dashboard", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false); dashboard = canvasObject.GetComponent<Canvas>(); dashboard.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600,900); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            var background = Panel("Paper background", dashboard.transform, 0,0,1600,900,Hex(0xF2F0E7));
            background.anchorMin = Vector2.zero; background.anchorMax = Vector2.one; background.offsetMin = background.offsetMax = Vector2.zero;
            var root = Rect("Election content", dashboard.transform, 0,0,1600,900); root.anchorMin = root.anchorMax = new Vector2(0.5f,0.5f); root.pivot = new Vector2(0.5f,0.5f); root.anchoredPosition = Vector2.zero;
            Label("Brand", root, 64,38,600,24,"TRAFFIC CONTROL  /  ГОРОДСКОЙ СОВЕТ",15,Muted,true);
            Label("Title",root,64,80,1100,64,"Выборы мэра",52,Ink,true);
            Label("Subtitle",root,66,151,1100,32,"Доверие жителей. Один город. Следующий срок.",20,Muted);
            phaseLabel = Label("Stage",root,1160,95,375,32,"01 / ПОДГОТОВКА",16,Green,true,TextAnchor.MiddleRight);
            Panel("Header rule",root,64,205,1472,2,Hex(0xD9DED3));

            var mayorCard=Panel("Mayor card",root,64,237,490,376,Ink);
            Label("Candidate tag",mayorCard,30,24,420,24,"ВАШ КАНДИДАТ",14,Hex(0xA4C8B4),true);
            Label("Candidate",mayorCard,30,62,420,48,"Действующий мэр",31,Color.white,true);
            Label("Share caption",mayorCard,30,125,420,24,"Доля среди поданных голосов",17,Hex(0xBDD0C5));
            shareNumber=Label("Vote share",mayorCard,26,166,430,100,"52,5%",80,Hex(0xE3C681),true);
            Panel("Candidate rule",mayorCard,30,284,430,1,Hex(0x49665C));
            Label("Candidate footer",mayorCard,30,305,430,46,"Ваши решения формируют\nследующую главу города.",18,Hex(0xD1DED4));

            var votes=Panel("Vote allocation",root,578,237,958,376,Color.white);
            Label("Allocation heading",votes,30,24,880,30,"Распределение голосов",24,Ink,true);
            turnoutLabel=Label("Turnout",votes,30,64,880,28,"",16,Muted);
            VoteRow(votes,"Mayor",30,115,"За действующего мэра",Green,out mayorNumber,out mayorPercent,out mayorBar,out mayorMinus,out mayorPlus);
            VoteRow(votes,"Opponent",30,227,"За соперника",Gold,out opponentNumber,out opponentPercent,out opponentBar,out opponentMinus,out opponentPlus);

            var undecided=Panel("Unassigned votes",root,64,635,490,62,Hex(0xE5E9DF));
            undecidedCaption=Label("Caption",undecided,20,10,300,20,"Ещё не назначенные голоса",16,Muted);
            undecidedNumber=Label("Number",undecided,325,10,145,42,"2 000",30,Ink,true,TextAnchor.MiddleRight);

            var results=Panel("Election result",root,578,635,958,190,Color.white);
            verdictStripe=Panel("Result accent",results,0,0,5,190,Green).GetComponent<Image>();
            verdictBadge=Label("Verdict tag",results,28,18,900,23,"",13,Green,true);
            resultHeading=Label("Result title",results,28,50,900,48,"Город решает",33,Ink,true);
            resultBody=Label("Result description",results,30,108,895,67,"",18,Muted);

            countLabel=Label("Count caption",root,66,787,484,22,"",12,Muted,true);
            var track=Panel("Count track",root,64,820,490,5,Hex(0xD7DDD1));
            countFill=Panel("Count progress",track,0,0,0,5,Green).GetComponent<Image>();

            primary=Button("Run election",root,1090,843,446,43,"Провести выборы     Enter",Green,Color.white,out actionLabel);
            reset=Button("Reset",root,860,843,206,43,"Сбросить     R",Hex(0xE4E7DE),Ink,out _);
            Label("Keys",root,64,850,770,24,"Мэр: ↑ ↓   ·   Соперник: ← →   ·   Шаг 100 / Shift: 500",15,Muted);
            noteLabel=Label("Rules",root,66,744,488,36,"",13,Muted);
            winPreset=Button("Win preset",root,64,704,110,34,"1 Победа",Hex(0xD2DDCC),Ink,out _);
            losePreset=Button("Lose preset",root,184,704,120,34,"2 Проигрыш",Hex(0xD2DDCC),Ink,out _);
            tiePreset=Button("Tie preset",root,314,704,115,34,"3 Ничья",Hex(0xD2DDCC),Ink,out _);
            emptyPreset=Button("Empty preset",root,439,704,90,34,"0 Пусто",Hex(0xD2DDCC),Ink,out _);
            if (session == null) session = new ElectionSession();
            Refresh();
        }
        private void VoteRow(Transform parent,string name,float x,float y,string title,Color color,out Text number,out Text percent,out Image bar,out Button minus,out Button plus)
        {
            var row=Rect(name+" row",parent,x,y,900,94);
            Label("Label",row,0,0,560,25,title,18,Ink,true);
            number=Label("Votes",row,0,29,290,50,"",37,Ink,true);
            percent=Label("Percentage",row,300,43,340,26,"",16,Muted);
            var track=Panel("Bar track",row,0,87,900,5,Hex(0xEBEEE6));
            bar=Panel("Bar",track,0,0,0,5,color).GetComponent<Image>();
            minus=Button("Minus",row,688,14,90,48,"−100",Hex(0xEEF1E9),Ink,out _);
            plus=Button("Plus",row,794,14,106,48,"+100",color,Color.white,out _);
        }
        private static void SetBar(Image image,float fraction,float fullWidth)
        { image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,Mathf.Clamp01(fraction)*fullWidth); }
        private Text Label(string name,Transform parent,float x,float y,float width,float height,string value,int size,Color color,bool bold=false,TextAnchor alignment=TextAnchor.UpperLeft)
        {
            var go=Rect(name,parent,x,y,width,height); var text=go.gameObject.AddComponent<Text>(); text.font=bold?BoldFont:RegularFont;
            text.text=value; text.fontSize=size; text.color=color; text.alignment=alignment; text.raycastTarget=false;
            text.horizontalOverflow=HorizontalWrapMode.Wrap; text.verticalOverflow=VerticalWrapMode.Truncate; return text;
        }
        private Button Button(string name,Transform parent,float x,float y,float width,float height,string value,Color background,Color foreground,out Text label)
        {
            var rect=Panel(name,parent,x,y,width,height,background); var button=rect.gameObject.AddComponent<Button>();
            var colors=button.colors; colors.highlightedColor=new Color(0.91f,0.95f,0.89f); colors.pressedColor=new Color(0.74f,0.82f,0.73f); colors.disabledColor=new Color(0.75f,0.77f,0.73f,0.55f); button.colors=colors;
            button.targetGraphic=rect.GetComponent<Image>(); button.navigation=new Navigation { mode=Navigation.Mode.None };
            label=Label("Label",rect,0,0,width,height,value,16,foreground,true,TextAnchor.MiddleCenter); return button;
        }
        private static RectTransform Panel(string name,Transform parent,float x,float y,float width,float height,Color color)
        { var rect=Rect(name,parent,x,y,width,height); rect.gameObject.AddComponent<Image>().color=color; return rect; }
        private static RectTransform Rect(string name,Transform parent,float x,float y,float width,float height)
        { var go=new GameObject(name,typeof(RectTransform)); var rect=go.GetComponent<RectTransform>(); rect.SetParent(parent,false); Place(rect,x,y,width,height); return rect; }
        private static void Place(RectTransform rect,float x,float y,float width,float height)
        { rect.anchorMin=rect.anchorMax=new Vector2(0,1); rect.pivot=new Vector2(0,1); rect.anchoredPosition=new Vector2(x,-y); rect.sizeDelta=new Vector2(width,height); }
        private static Color Hex(uint rgb) => new Color(((rgb>>16)&255)/255f,((rgb>>8)&255)/255f,(rgb&255)/255f,1);
    }
}
