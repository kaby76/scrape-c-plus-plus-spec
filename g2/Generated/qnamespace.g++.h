# 44 "/usr/include/x86_64-linux-gnu/qt5/QtCore/qnamespace.h" 2








struct QMetaObject;
const QMetaObject *qt_getQtMetaObject() noexcept;
# 64 "/usr/include/x86_64-linux-gnu/qt5/QtCore/qnamespace.h"
namespace



Qt {






    enum GlobalColor {
        color0,
        color1,
        black,
        white,
        darkGray,
        gray,
        lightGray,
        red,
        green,
        blue,
        cyan,
        magenta,
        yellow,
        darkRed,
        darkGreen,
        darkBlue,
        darkCyan,
        darkMagenta,
        darkYellow,
        transparent
    };

    enum KeyboardModifier {
        NoModifier = 0x00000000,
        ShiftModifier = 0x02000000,
        ControlModifier = 0x04000000,
        AltModifier = 0x08000000,
        MetaModifier = 0x10000000,
        KeypadModifier = 0x20000000,
        GroupSwitchModifier = 0x40000000,

        KeyboardModifierMask = 0xfe000000
    };
    typedef QFlags<KeyboardModifier> KeyboardModifiers;
    constexpr inline QFlags<KeyboardModifiers::enum_type> operator|(KeyboardModifiers::enum_type f1, KeyboardModifiers::enum_type f2) noexcept { return QFlags<KeyboardModifiers::enum_type>(f1) | f2; } constexpr inline QFlags<KeyboardModifiers::enum_type> operator|(KeyboardModifiers::enum_type f1, QFlags<KeyboardModifiers::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(KeyboardModifiers::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }






    enum Modifier {
        META = Qt::MetaModifier,
        SHIFT = Qt::ShiftModifier,
        CTRL = Qt::ControlModifier,
        ALT = Qt::AltModifier,
        MODIFIER_MASK = KeyboardModifierMask,
        UNICODE_ACCEL = 0x00000000
    };

    enum MouseButton {
        NoButton = 0x00000000,
        LeftButton = 0x00000001,
        RightButton = 0x00000002,
        MidButton = 0x00000004,
        MiddleButton = MidButton,
        BackButton = 0x00000008,
        XButton1 = BackButton,
        ExtraButton1 = XButton1,
        ForwardButton = 0x00000010,
        XButton2 = ForwardButton,
        ExtraButton2 = ForwardButton,
        TaskButton = 0x00000020,
        ExtraButton3 = TaskButton,
        ExtraButton4 = 0x00000040,
        ExtraButton5 = 0x00000080,
        ExtraButton6 = 0x00000100,
        ExtraButton7 = 0x00000200,
        ExtraButton8 = 0x00000400,
        ExtraButton9 = 0x00000800,
        ExtraButton10 = 0x00001000,
        ExtraButton11 = 0x00002000,
        ExtraButton12 = 0x00004000,
        ExtraButton13 = 0x00008000,
        ExtraButton14 = 0x00010000,
        ExtraButton15 = 0x00020000,
        ExtraButton16 = 0x00040000,
        ExtraButton17 = 0x00080000,
        ExtraButton18 = 0x00100000,
        ExtraButton19 = 0x00200000,
        ExtraButton20 = 0x00400000,
        ExtraButton21 = 0x00800000,
        ExtraButton22 = 0x01000000,
        ExtraButton23 = 0x02000000,
        ExtraButton24 = 0x04000000,
        AllButtons = 0x07ffffff,
        MaxMouseButton = ExtraButton24,

        MouseButtonMask = 0xffffffff
    };
    typedef QFlags<MouseButton> MouseButtons;
    constexpr inline QFlags<MouseButtons::enum_type> operator|(MouseButtons::enum_type f1, MouseButtons::enum_type f2) noexcept { return QFlags<MouseButtons::enum_type>(f1) | f2; } constexpr inline QFlags<MouseButtons::enum_type> operator|(MouseButtons::enum_type f1, QFlags<MouseButtons::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(MouseButtons::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum Orientation {
        Horizontal = 0x1,
        Vertical = 0x2
    };

    typedef QFlags<Orientation> Orientations;
    constexpr inline QFlags<Orientations::enum_type> operator|(Orientations::enum_type f1, Orientations::enum_type f2) noexcept { return QFlags<Orientations::enum_type>(f1) | f2; } constexpr inline QFlags<Orientations::enum_type> operator|(Orientations::enum_type f1, QFlags<Orientations::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(Orientations::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum FocusPolicy {
        NoFocus = 0,
        TabFocus = 0x1,
        ClickFocus = 0x2,
        StrongFocus = TabFocus | ClickFocus | 0x8,
        WheelFocus = StrongFocus | 0x4
    };

    enum TabFocusBehavior {
        NoTabFocus = 0x00,
        TabFocusTextControls = 0x01,
        TabFocusListControls = 0x02,
        TabFocusAllControls = 0xff
    };

    enum SortOrder {
        AscendingOrder,
        DescendingOrder
    };

    enum TileRule {
        StretchTile,
        RepeatTile,
        RoundTile
    };





    enum AlignmentFlag {
        AlignLeft = 0x0001,
        AlignLeading = AlignLeft,
        AlignRight = 0x0002,
        AlignTrailing = AlignRight,
        AlignHCenter = 0x0004,
        AlignJustify = 0x0008,
        AlignAbsolute = 0x0010,
        AlignHorizontal_Mask = AlignLeft | AlignRight | AlignHCenter | AlignJustify | AlignAbsolute,

        AlignTop = 0x0020,
        AlignBottom = 0x0040,
        AlignVCenter = 0x0080,
        AlignBaseline = 0x0100,




        AlignVertical_Mask = AlignTop | AlignBottom | AlignVCenter | AlignBaseline,

        AlignCenter = AlignVCenter | AlignHCenter
    };

    typedef QFlags<AlignmentFlag> Alignment;
    constexpr inline QFlags<Alignment::enum_type> operator|(Alignment::enum_type f1, Alignment::enum_type f2) noexcept { return QFlags<Alignment::enum_type>(f1) | f2; } constexpr inline QFlags<Alignment::enum_type> operator|(Alignment::enum_type f1, QFlags<Alignment::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(Alignment::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum TextFlag {
        TextSingleLine = 0x0100,
        TextDontClip = 0x0200,
        TextExpandTabs = 0x0400,
        TextShowMnemonic = 0x0800,
        TextWordWrap = 0x1000,
        TextWrapAnywhere = 0x2000,
        TextDontPrint = 0x4000,
        TextIncludeTrailingSpaces = 0x08000000,
        TextHideMnemonic = 0x8000,
        TextJustificationForced = 0x10000,
        TextForceLeftToRight = 0x20000,
        TextForceRightToLeft = 0x40000,


        TextLongestVariant = 0x80000


        , TextBypassShaping = 0x100000

    };

    enum TextElideMode {
        ElideLeft,
        ElideRight,
        ElideMiddle,
        ElideNone
    };

    enum WhiteSpaceMode {
        WhiteSpaceNormal,
        WhiteSpacePre,
        WhiteSpaceNoWrap,
        WhiteSpaceModeUndefined = -1
    };

    enum HitTestAccuracy { ExactHit, FuzzyHit };

    enum WindowType {
        Widget = 0x00000000,
        Window = 0x00000001,
        Dialog = 0x00000002 | Window,
        Sheet = 0x00000004 | Window,
        Drawer = Sheet | Dialog,
        Popup = 0x00000008 | Window,
        Tool = Popup | Dialog,
        ToolTip = Popup | Sheet,
        SplashScreen = ToolTip | Dialog,
        Desktop = 0x00000010 | Window,
        SubWindow = 0x00000012,
        ForeignWindow = 0x00000020 | Window,
        CoverWindow = 0x00000040 | Window,

        WindowType_Mask = 0x000000ff,
        MSWindowsFixedSizeDialogHint = 0x00000100,
        MSWindowsOwnDC = 0x00000200,
        BypassWindowManagerHint = 0x00000400,
        X11BypassWindowManagerHint = BypassWindowManagerHint,
        FramelessWindowHint = 0x00000800,
        WindowTitleHint = 0x00001000,
        WindowSystemMenuHint = 0x00002000,
        WindowMinimizeButtonHint = 0x00004000,
        WindowMaximizeButtonHint = 0x00008000,
        WindowMinMaxButtonsHint = WindowMinimizeButtonHint | WindowMaximizeButtonHint,
        WindowContextHelpButtonHint = 0x00010000,
        WindowShadeButtonHint = 0x00020000,
        WindowStaysOnTopHint = 0x00040000,
        WindowTransparentForInput = 0x00080000,
        WindowOverridesSystemGestures = 0x00100000,
        WindowDoesNotAcceptFocus = 0x00200000,
        MaximizeUsingFullscreenGeometryHint = 0x00400000,

        CustomizeWindowHint = 0x02000000,
        WindowStaysOnBottomHint = 0x04000000,
        WindowCloseButtonHint = 0x08000000,
        MacWindowToolBarButtonHint = 0x10000000,
        BypassGraphicsProxyWidget = 0x20000000,
        NoDropShadowWindowHint = 0x40000000,
        WindowFullscreenButtonHint = 0x80000000
    };

    typedef QFlags<WindowType> WindowFlags;
    constexpr inline QFlags<WindowFlags::enum_type> operator|(WindowFlags::enum_type f1, WindowFlags::enum_type f2) noexcept { return QFlags<WindowFlags::enum_type>(f1) | f2; } constexpr inline QFlags<WindowFlags::enum_type> operator|(WindowFlags::enum_type f1, QFlags<WindowFlags::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(WindowFlags::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum WindowState {
        WindowNoState = 0x00000000,
        WindowMinimized = 0x00000001,
        WindowMaximized = 0x00000002,
        WindowFullScreen = 0x00000004,
        WindowActive = 0x00000008
    };

    typedef QFlags<WindowState> WindowStates;
    constexpr inline QFlags<WindowStates::enum_type> operator|(WindowStates::enum_type f1, WindowStates::enum_type f2) noexcept { return QFlags<WindowStates::enum_type>(f1) | f2; } constexpr inline QFlags<WindowStates::enum_type> operator|(WindowStates::enum_type f1, QFlags<WindowStates::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(WindowStates::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum ApplicationState {
        ApplicationSuspended = 0x00000000,
        ApplicationHidden = 0x00000001,
        ApplicationInactive = 0x00000002,
        ApplicationActive = 0x00000004
    };

    typedef QFlags<ApplicationState> ApplicationStates;

    enum ScreenOrientation {
        PrimaryOrientation = 0x00000000,
        PortraitOrientation = 0x00000001,
        LandscapeOrientation = 0x00000002,
        InvertedPortraitOrientation = 0x00000004,
        InvertedLandscapeOrientation = 0x00000008
    };

    typedef QFlags<ScreenOrientation> ScreenOrientations;
    constexpr inline QFlags<ScreenOrientations::enum_type> operator|(ScreenOrientations::enum_type f1, ScreenOrientations::enum_type f2) noexcept { return QFlags<ScreenOrientations::enum_type>(f1) | f2; } constexpr inline QFlags<ScreenOrientations::enum_type> operator|(ScreenOrientations::enum_type f1, QFlags<ScreenOrientations::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(ScreenOrientations::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum WidgetAttribute {
        WA_Disabled = 0,
        WA_UnderMouse = 1,
        WA_MouseTracking = 2,
        WA_ContentsPropagated = 3,
        WA_OpaquePaintEvent = 4,
        WA_NoBackground = WA_OpaquePaintEvent,
        WA_StaticContents = 5,
        WA_LaidOut = 7,
        WA_PaintOnScreen = 8,
        WA_NoSystemBackground = 9,
        WA_UpdatesDisabled = 10,
        WA_Mapped = 11,
        WA_MacNoClickThrough = 12,
        WA_InputMethodEnabled = 14,
        WA_WState_Visible = 15,
        WA_WState_Hidden = 16,

        WA_ForceDisabled = 32,
        WA_KeyCompression = 33,
        WA_PendingMoveEvent = 34,
        WA_PendingResizeEvent = 35,
        WA_SetPalette = 36,
        WA_SetFont = 37,
        WA_SetCursor = 38,
        WA_NoChildEventsFromChildren = 39,
        WA_WindowModified = 41,
        WA_Resized = 42,
        WA_Moved = 43,
        WA_PendingUpdate = 44,
        WA_InvalidSize = 45,
        WA_MacBrushedMetal = 46,
        WA_MacMetalStyle = WA_MacBrushedMetal,
        WA_CustomWhatsThis = 47,
        WA_LayoutOnEntireRect = 48,
        WA_OutsideWSRange = 49,
        WA_GrabbedShortcut = 50,
        WA_TransparentForMouseEvents = 51,
        WA_PaintUnclipped = 52,
        WA_SetWindowIcon = 53,
        WA_NoMouseReplay = 54,
        WA_DeleteOnClose = 55,
        WA_RightToLeft = 56,
        WA_SetLayoutDirection = 57,
        WA_NoChildEventsForParent = 58,
        WA_ForceUpdatesDisabled = 59,

        WA_WState_Created = 60,
        WA_WState_CompressKeys = 61,
        WA_WState_InPaintEvent = 62,
        WA_WState_Reparented = 63,
        WA_WState_ConfigPending = 64,
        WA_WState_Polished = 66,
        WA_WState_DND = 67,
        WA_WState_OwnSizePolicy = 68,
        WA_WState_ExplicitShowHide = 69,

        WA_ShowModal = 70,
        WA_MouseNoMask = 71,
        WA_GroupLeader = 72,
        WA_NoMousePropagation = 73,
        WA_Hover = 74,
        WA_InputMethodTransparent = 75,
        WA_QuitOnClose = 76,

        WA_KeyboardFocusChange = 77,

        WA_AcceptDrops = 78,
        WA_DropSiteRegistered = 79,
        WA_ForceAcceptDrops = WA_DropSiteRegistered,

        WA_WindowPropagation = 80,

        WA_NoX11EventCompression = 81,
        WA_TintedBackground = 82,
        WA_X11OpenGLOverlay = 83,
        WA_AlwaysShowToolTips = 84,
        WA_MacOpaqueSizeGrip = 85,
        WA_SetStyle = 86,

        WA_SetLocale = 87,
        WA_MacShowFocusRect = 88,

        WA_MacNormalSize = 89,
        WA_MacSmallSize = 90,
        WA_MacMiniSize = 91,

        WA_LayoutUsesWidgetRect = 92,
        WA_StyledBackground = 93,
        WA_MSWindowsUseDirect3D = 94,
        WA_CanHostQMdiSubWindowTitleBar = 95,

        WA_MacAlwaysShowToolWindow = 96,

        WA_StyleSheet = 97,

        WA_ShowWithoutActivating = 98,

        WA_X11BypassTransientForHint = 99,

        WA_NativeWindow = 100,
        WA_DontCreateNativeAncestors = 101,

        WA_MacVariableSize = 102,

        WA_DontShowOnScreen = 103,


        WA_X11NetWmWindowTypeDesktop = 104,
        WA_X11NetWmWindowTypeDock = 105,
        WA_X11NetWmWindowTypeToolBar = 106,
        WA_X11NetWmWindowTypeMenu = 107,
        WA_X11NetWmWindowTypeUtility = 108,
        WA_X11NetWmWindowTypeSplash = 109,
        WA_X11NetWmWindowTypeDialog = 110,
        WA_X11NetWmWindowTypeDropDownMenu = 111,
        WA_X11NetWmWindowTypePopupMenu = 112,
        WA_X11NetWmWindowTypeToolTip = 113,
        WA_X11NetWmWindowTypeNotification = 114,
        WA_X11NetWmWindowTypeCombo = 115,
        WA_X11NetWmWindowTypeDND = 116,

        WA_MacFrameworkScaled = 117,

        WA_SetWindowModality = 118,
        WA_WState_WindowOpacitySet = 119,
        WA_TranslucentBackground = 120,

        WA_AcceptTouchEvents = 121,
        WA_WState_AcceptedTouchBeginEvent = 122,
        WA_TouchPadAcceptSingleTouchEvents = 123,

        WA_X11DoNotAcceptFocus = 126,
        WA_MacNoShadow = 127,

        WA_AlwaysStackOnTop = 128,

        WA_TabletTracking = 129,

        WA_ContentsMarginsRespectsSafeArea = 130,

        WA_StyleSheetTarget = 131,


        WA_AttributeCount
    };

    enum ApplicationAttribute
    {
        AA_ImmediateWidgetCreation = 0,
        AA_MSWindowsUseDirect3DByDefault = 1,
        AA_DontShowIconsInMenus = 2,
        AA_NativeWindows = 3,
        AA_DontCreateNativeWidgetSiblings = 4,
        AA_PluginApplication = 5,
        AA_MacPluginApplication = AA_PluginApplication,
        AA_DontUseNativeMenuBar = 6,
        AA_MacDontSwapCtrlAndMeta = 7,
        AA_Use96Dpi = 8,
        AA_X11InitThreads = 10,
        AA_SynthesizeTouchForUnhandledMouseEvents = 11,
        AA_SynthesizeMouseForUnhandledTouchEvents = 12,
        AA_UseHighDpiPixmaps = 13,
        AA_ForceRasterWidgets = 14,
        AA_UseDesktopOpenGL = 15,
        AA_UseOpenGLES = 16,
        AA_UseSoftwareOpenGL = 17,
        AA_ShareOpenGLContexts = 18,
        AA_SetPalette = 19,
        AA_EnableHighDpiScaling = 20,
        AA_DisableHighDpiScaling = 21,
        AA_UseStyleSheetPropagationInWidgetStyles = 22,
        AA_DontUseNativeDialogs = 23,
        AA_SynthesizeMouseForUnhandledTabletEvents = 24,
        AA_CompressHighFrequencyEvents = 25,
        AA_DontCheckOpenGLContextThreadAffinity = 26,
        AA_DisableShaderDiskCache = 27,
        AA_DontShowShortcutsInContextMenus = 28,
        AA_CompressTabletEvents = 29,
        AA_DisableWindowContextHelpButton = 30,


        AA_AttributeCount
    };





    enum ImageConversionFlag {
        ColorMode_Mask = 0x00000003,
        AutoColor = 0x00000000,
        ColorOnly = 0x00000003,
        MonoOnly = 0x00000002,


        AlphaDither_Mask = 0x0000000c,
        ThresholdAlphaDither = 0x00000000,
        OrderedAlphaDither = 0x00000004,
        DiffuseAlphaDither = 0x00000008,
        NoAlpha = 0x0000000c,

        Dither_Mask = 0x00000030,
        DiffuseDither = 0x00000000,
        OrderedDither = 0x00000010,
        ThresholdDither = 0x00000020,


        DitherMode_Mask = 0x000000c0,
        AutoDither = 0x00000000,
        PreferDither = 0x00000040,
        AvoidDither = 0x00000080,

        NoOpaqueDetection = 0x00000100,
        NoFormatConversion = 0x00000200
    };
    typedef QFlags<ImageConversionFlag> ImageConversionFlags;
    constexpr inline QFlags<ImageConversionFlags::enum_type> operator|(ImageConversionFlags::enum_type f1, ImageConversionFlags::enum_type f2) noexcept { return QFlags<ImageConversionFlags::enum_type>(f1) | f2; } constexpr inline QFlags<ImageConversionFlags::enum_type> operator|(ImageConversionFlags::enum_type f1, QFlags<ImageConversionFlags::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(ImageConversionFlags::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum BGMode {
        TransparentMode,
        OpaqueMode
    };

    enum Key {
        Key_Escape = 0x01000000,
        Key_Tab = 0x01000001,
        Key_Backtab = 0x01000002,
        Key_Backspace = 0x01000003,
        Key_Return = 0x01000004,
        Key_Enter = 0x01000005,
        Key_Insert = 0x01000006,
        Key_Delete = 0x01000007,
        Key_Pause = 0x01000008,
        Key_Print = 0x01000009,
        Key_SysReq = 0x0100000a,
        Key_Clear = 0x0100000b,
        Key_Home = 0x01000010,
        Key_End = 0x01000011,
        Key_Left = 0x01000012,
        Key_Up = 0x01000013,
        Key_Right = 0x01000014,
        Key_Down = 0x01000015,
        Key_PageUp = 0x01000016,
        Key_PageDown = 0x01000017,
        Key_Shift = 0x01000020,
        Key_Control = 0x01000021,
        Key_Meta = 0x01000022,
        Key_Alt = 0x01000023,
        Key_CapsLock = 0x01000024,
        Key_NumLock = 0x01000025,
        Key_ScrollLock = 0x01000026,
        Key_F1 = 0x01000030,
        Key_F2 = 0x01000031,
        Key_F3 = 0x01000032,
        Key_F4 = 0x01000033,
        Key_F5 = 0x01000034,
        Key_F6 = 0x01000035,
        Key_F7 = 0x01000036,
        Key_F8 = 0x01000037,
        Key_F9 = 0x01000038,
        Key_F10 = 0x01000039,
        Key_F11 = 0x0100003a,
        Key_F12 = 0x0100003b,
        Key_F13 = 0x0100003c,
        Key_F14 = 0x0100003d,
        Key_F15 = 0x0100003e,
        Key_F16 = 0x0100003f,
        Key_F17 = 0x01000040,
        Key_F18 = 0x01000041,
        Key_F19 = 0x01000042,
        Key_F20 = 0x01000043,
        Key_F21 = 0x01000044,
        Key_F22 = 0x01000045,
        Key_F23 = 0x01000046,
        Key_F24 = 0x01000047,
        Key_F25 = 0x01000048,
        Key_F26 = 0x01000049,
        Key_F27 = 0x0100004a,
        Key_F28 = 0x0100004b,
        Key_F29 = 0x0100004c,
        Key_F30 = 0x0100004d,
        Key_F31 = 0x0100004e,
        Key_F32 = 0x0100004f,
        Key_F33 = 0x01000050,
        Key_F34 = 0x01000051,
        Key_F35 = 0x01000052,
        Key_Super_L = 0x01000053,
        Key_Super_R = 0x01000054,
        Key_Menu = 0x01000055,
        Key_Hyper_L = 0x01000056,
        Key_Hyper_R = 0x01000057,
        Key_Help = 0x01000058,
        Key_Direction_L = 0x01000059,
        Key_Direction_R = 0x01000060,
        Key_Space = 0x20,
        Key_Any = Key_Space,
        Key_Exclam = 0x21,
        Key_QuoteDbl = 0x22,
        Key_NumberSign = 0x23,
        Key_Dollar = 0x24,
        Key_Percent = 0x25,
        Key_Ampersand = 0x26,
        Key_Apostrophe = 0x27,
        Key_ParenLeft = 0x28,
        Key_ParenRight = 0x29,
        Key_Asterisk = 0x2a,
        Key_Plus = 0x2b,
        Key_Comma = 0x2c,
        Key_Minus = 0x2d,
        Key_Period = 0x2e,
        Key_Slash = 0x2f,
        Key_0 = 0x30,
        Key_1 = 0x31,
        Key_2 = 0x32,
        Key_3 = 0x33,
        Key_4 = 0x34,
        Key_5 = 0x35,
        Key_6 = 0x36,
        Key_7 = 0x37,
        Key_8 = 0x38,
        Key_9 = 0x39,
        Key_Colon = 0x3a,
        Key_Semicolon = 0x3b,
        Key_Less = 0x3c,
        Key_Equal = 0x3d,
        Key_Greater = 0x3e,
        Key_Question = 0x3f,
        Key_At = 0x40,
        Key_A = 0x41,
        Key_B = 0x42,
        Key_C = 0x43,
        Key_D = 0x44,
        Key_E = 0x45,
        Key_F = 0x46,
        Key_G = 0x47,
        Key_H = 0x48,
        Key_I = 0x49,
        Key_J = 0x4a,
        Key_K = 0x4b,
        Key_L = 0x4c,
        Key_M = 0x4d,
        Key_N = 0x4e,
        Key_O = 0x4f,
        Key_P = 0x50,
        Key_Q = 0x51,
        Key_R = 0x52,
        Key_S = 0x53,
        Key_T = 0x54,
        Key_U = 0x55,
        Key_V = 0x56,
        Key_W = 0x57,
        Key_X = 0x58,
        Key_Y = 0x59,
        Key_Z = 0x5a,
        Key_BracketLeft = 0x5b,
        Key_Backslash = 0x5c,
        Key_BracketRight = 0x5d,
        Key_AsciiCircum = 0x5e,
        Key_Underscore = 0x5f,
        Key_QuoteLeft = 0x60,
        Key_BraceLeft = 0x7b,
        Key_Bar = 0x7c,
        Key_BraceRight = 0x7d,
        Key_AsciiTilde = 0x7e,

        Key_nobreakspace = 0x0a0,
        Key_exclamdown = 0x0a1,
        Key_cent = 0x0a2,
        Key_sterling = 0x0a3,
        Key_currency = 0x0a4,
        Key_yen = 0x0a5,
        Key_brokenbar = 0x0a6,
        Key_section = 0x0a7,
        Key_diaeresis = 0x0a8,
        Key_copyright = 0x0a9,
        Key_ordfeminine = 0x0aa,
        Key_guillemotleft = 0x0ab,
        Key_notsign = 0x0ac,
        Key_hyphen = 0x0ad,
        Key_registered = 0x0ae,
        Key_macron = 0x0af,
        Key_degree = 0x0b0,
        Key_plusminus = 0x0b1,
        Key_twosuperior = 0x0b2,
        Key_threesuperior = 0x0b3,
        Key_acute = 0x0b4,
        Key_mu = 0x0b5,
        Key_paragraph = 0x0b6,
        Key_periodcentered = 0x0b7,
        Key_cedilla = 0x0b8,
        Key_onesuperior = 0x0b9,
        Key_masculine = 0x0ba,
        Key_guillemotright = 0x0bb,
        Key_onequarter = 0x0bc,
        Key_onehalf = 0x0bd,
        Key_threequarters = 0x0be,
        Key_questiondown = 0x0bf,
        Key_Agrave = 0x0c0,
        Key_Aacute = 0x0c1,
        Key_Acircumflex = 0x0c2,
        Key_Atilde = 0x0c3,
        Key_Adiaeresis = 0x0c4,
        Key_Aring = 0x0c5,
        Key_AE = 0x0c6,
        Key_Ccedilla = 0x0c7,
        Key_Egrave = 0x0c8,
        Key_Eacute = 0x0c9,
        Key_Ecircumflex = 0x0ca,
        Key_Ediaeresis = 0x0cb,
        Key_Igrave = 0x0cc,
        Key_Iacute = 0x0cd,
        Key_Icircumflex = 0x0ce,
        Key_Idiaeresis = 0x0cf,
        Key_ETH = 0x0d0,
        Key_Ntilde = 0x0d1,
        Key_Ograve = 0x0d2,
        Key_Oacute = 0x0d3,
        Key_Ocircumflex = 0x0d4,
        Key_Otilde = 0x0d5,
        Key_Odiaeresis = 0x0d6,
        Key_multiply = 0x0d7,
        Key_Ooblique = 0x0d8,
        Key_Ugrave = 0x0d9,
        Key_Uacute = 0x0da,
        Key_Ucircumflex = 0x0db,
        Key_Udiaeresis = 0x0dc,
        Key_Yacute = 0x0dd,
        Key_THORN = 0x0de,
        Key_ssharp = 0x0df,
        Key_division = 0x0f7,
        Key_ydiaeresis = 0x0ff,






        Key_AltGr = 0x01001103,
        Key_Multi_key = 0x01001120,
        Key_Codeinput = 0x01001137,
        Key_SingleCandidate = 0x0100113c,
        Key_MultipleCandidate = 0x0100113d,
        Key_PreviousCandidate = 0x0100113e,


        Key_Mode_switch = 0x0100117e,



        Key_Kanji = 0x01001121,
        Key_Muhenkan = 0x01001122,

        Key_Henkan = 0x01001123,
        Key_Romaji = 0x01001124,
        Key_Hiragana = 0x01001125,
        Key_Katakana = 0x01001126,
        Key_Hiragana_Katakana = 0x01001127,
        Key_Zenkaku = 0x01001128,
        Key_Hankaku = 0x01001129,
        Key_Zenkaku_Hankaku = 0x0100112a,
        Key_Touroku = 0x0100112b,
        Key_Massyo = 0x0100112c,
        Key_Kana_Lock = 0x0100112d,
        Key_Kana_Shift = 0x0100112e,
        Key_Eisu_Shift = 0x0100112f,
        Key_Eisu_toggle = 0x01001130,
# 824 "/usr/include/x86_64-linux-gnu/qt5/QtCore/qnamespace.h"
        Key_Hangul = 0x01001131,
        Key_Hangul_Start = 0x01001132,
        Key_Hangul_End = 0x01001133,
        Key_Hangul_Hanja = 0x01001134,
        Key_Hangul_Jamo = 0x01001135,
        Key_Hangul_Romaja = 0x01001136,

        Key_Hangul_Jeonja = 0x01001138,
        Key_Hangul_Banja = 0x01001139,
        Key_Hangul_PreHanja = 0x0100113a,
        Key_Hangul_PostHanja = 0x0100113b,



        Key_Hangul_Special = 0x0100113f,



        Key_Dead_Grave = 0x01001250,
        Key_Dead_Acute = 0x01001251,
        Key_Dead_Circumflex = 0x01001252,
        Key_Dead_Tilde = 0x01001253,
        Key_Dead_Macron = 0x01001254,
        Key_Dead_Breve = 0x01001255,
        Key_Dead_Abovedot = 0x01001256,
        Key_Dead_Diaeresis = 0x01001257,
        Key_Dead_Abovering = 0x01001258,
        Key_Dead_Doubleacute = 0x01001259,
        Key_Dead_Caron = 0x0100125a,
        Key_Dead_Cedilla = 0x0100125b,
        Key_Dead_Ogonek = 0x0100125c,
        Key_Dead_Iota = 0x0100125d,
        Key_Dead_Voiced_Sound = 0x0100125e,
        Key_Dead_Semivoiced_Sound = 0x0100125f,
        Key_Dead_Belowdot = 0x01001260,
        Key_Dead_Hook = 0x01001261,
        Key_Dead_Horn = 0x01001262,
        Key_Dead_Stroke = 0x01001263,
        Key_Dead_Abovecomma = 0x01001264,
        Key_Dead_Abovereversedcomma = 0x01001265,
        Key_Dead_Doublegrave = 0x01001266,
        Key_Dead_Belowring = 0x01001267,
        Key_Dead_Belowmacron = 0x01001268,
        Key_Dead_Belowcircumflex = 0x01001269,
        Key_Dead_Belowtilde = 0x0100126a,
        Key_Dead_Belowbreve = 0x0100126b,
        Key_Dead_Belowdiaeresis = 0x0100126c,
        Key_Dead_Invertedbreve = 0x0100126d,
        Key_Dead_Belowcomma = 0x0100126e,
        Key_Dead_Currency = 0x0100126f,
        Key_Dead_a = 0x01001280,
        Key_Dead_A = 0x01001281,
        Key_Dead_e = 0x01001282,
        Key_Dead_E = 0x01001283,
        Key_Dead_i = 0x01001284,
        Key_Dead_I = 0x01001285,
        Key_Dead_o = 0x01001286,
        Key_Dead_O = 0x01001287,
        Key_Dead_u = 0x01001288,
        Key_Dead_U = 0x01001289,
        Key_Dead_Small_Schwa = 0x0100128a,
        Key_Dead_Capital_Schwa = 0x0100128b,
        Key_Dead_Greek = 0x0100128c,
        Key_Dead_Lowline = 0x01001290,
        Key_Dead_Aboveverticalline = 0x01001291,
        Key_Dead_Belowverticalline = 0x01001292,
        Key_Dead_Longsolidusoverlay = 0x01001293,


        Key_Back = 0x01000061,
        Key_Forward = 0x01000062,
        Key_Stop = 0x01000063,
        Key_Refresh = 0x01000064,
        Key_VolumeDown = 0x01000070,
        Key_VolumeMute = 0x01000071,
        Key_VolumeUp = 0x01000072,
        Key_BassBoost = 0x01000073,
        Key_BassUp = 0x01000074,
        Key_BassDown = 0x01000075,
        Key_TrebleUp = 0x01000076,
        Key_TrebleDown = 0x01000077,
        Key_MediaPlay = 0x01000080,
        Key_MediaStop = 0x01000081,
        Key_MediaPrevious = 0x01000082,
        Key_MediaNext = 0x01000083,
        Key_MediaRecord = 0x01000084,
        Key_MediaPause = 0x1000085,
        Key_MediaTogglePlayPause = 0x1000086,
        Key_HomePage = 0x01000090,
        Key_Favorites = 0x01000091,
        Key_Search = 0x01000092,
        Key_Standby = 0x01000093,
        Key_OpenUrl = 0x01000094,
        Key_LaunchMail = 0x010000a0,
        Key_LaunchMedia = 0x010000a1,
        Key_Launch0 = 0x010000a2,
        Key_Launch1 = 0x010000a3,
        Key_Launch2 = 0x010000a4,
        Key_Launch3 = 0x010000a5,
        Key_Launch4 = 0x010000a6,
        Key_Launch5 = 0x010000a7,
        Key_Launch6 = 0x010000a8,
        Key_Launch7 = 0x010000a9,
        Key_Launch8 = 0x010000aa,
        Key_Launch9 = 0x010000ab,
        Key_LaunchA = 0x010000ac,
        Key_LaunchB = 0x010000ad,
        Key_LaunchC = 0x010000ae,
        Key_LaunchD = 0x010000af,
        Key_LaunchE = 0x010000b0,
        Key_LaunchF = 0x010000b1,
        Key_MonBrightnessUp = 0x010000b2,
        Key_MonBrightnessDown = 0x010000b3,
        Key_KeyboardLightOnOff = 0x010000b4,
        Key_KeyboardBrightnessUp = 0x010000b5,
        Key_KeyboardBrightnessDown = 0x010000b6,
        Key_PowerOff = 0x010000b7,
        Key_WakeUp = 0x010000b8,
        Key_Eject = 0x010000b9,
        Key_ScreenSaver = 0x010000ba,
        Key_WWW = 0x010000bb,
        Key_Memo = 0x010000bc,
        Key_LightBulb = 0x010000bd,
        Key_Shop = 0x010000be,
        Key_History = 0x010000bf,
        Key_AddFavorite = 0x010000c0,
        Key_HotLinks = 0x010000c1,
        Key_BrightnessAdjust = 0x010000c2,
        Key_Finance = 0x010000c3,
        Key_Community = 0x010000c4,
        Key_AudioRewind = 0x010000c5,
        Key_BackForward = 0x010000c6,
        Key_ApplicationLeft = 0x010000c7,
        Key_ApplicationRight = 0x010000c8,
        Key_Book = 0x010000c9,
        Key_CD = 0x010000ca,
        Key_Calculator = 0x010000cb,
        Key_ToDoList = 0x010000cc,
        Key_ClearGrab = 0x010000cd,
        Key_Close = 0x010000ce,
        Key_Copy = 0x010000cf,
        Key_Cut = 0x010000d0,
        Key_Display = 0x010000d1,
        Key_DOS = 0x010000d2,
        Key_Documents = 0x010000d3,
        Key_Excel = 0x010000d4,
        Key_Explorer = 0x010000d5,
        Key_Game = 0x010000d6,
        Key_Go = 0x010000d7,
        Key_iTouch = 0x010000d8,
        Key_LogOff = 0x010000d9,
        Key_Market = 0x010000da,
        Key_Meeting = 0x010000db,
        Key_MenuKB = 0x010000dc,
        Key_MenuPB = 0x010000dd,
        Key_MySites = 0x010000de,
        Key_News = 0x010000df,
        Key_OfficeHome = 0x010000e0,
        Key_Option = 0x010000e1,
        Key_Paste = 0x010000e2,
        Key_Phone = 0x010000e3,
        Key_Calendar = 0x010000e4,
        Key_Reply = 0x010000e5,
        Key_Reload = 0x010000e6,
        Key_RotateWindows = 0x010000e7,
        Key_RotationPB = 0x010000e8,
        Key_RotationKB = 0x010000e9,
        Key_Save = 0x010000ea,
        Key_Send = 0x010000eb,
        Key_Spell = 0x010000ec,
        Key_SplitScreen = 0x010000ed,
        Key_Support = 0x010000ee,
        Key_TaskPane = 0x010000ef,
        Key_Terminal = 0x010000f0,
        Key_Tools = 0x010000f1,
        Key_Travel = 0x010000f2,
        Key_Video = 0x010000f3,
        Key_Word = 0x010000f4,
        Key_Xfer = 0x010000f5,
        Key_ZoomIn = 0x010000f6,
        Key_ZoomOut = 0x010000f7,
        Key_Away = 0x010000f8,
        Key_Messenger = 0x010000f9,
        Key_WebCam = 0x010000fa,
        Key_MailForward = 0x010000fb,
        Key_Pictures = 0x010000fc,
        Key_Music = 0x010000fd,
        Key_Battery = 0x010000fe,
        Key_Bluetooth = 0x010000ff,
        Key_WLAN = 0x01000100,
        Key_UWB = 0x01000101,
        Key_AudioForward = 0x01000102,
        Key_AudioRepeat = 0x01000103,
        Key_AudioRandomPlay = 0x01000104,
        Key_Subtitle = 0x01000105,
        Key_AudioCycleTrack = 0x01000106,
        Key_Time = 0x01000107,
        Key_Hibernate = 0x01000108,
        Key_View = 0x01000109,
        Key_TopMenu = 0x0100010a,
        Key_PowerDown = 0x0100010b,
        Key_Suspend = 0x0100010c,
        Key_ContrastAdjust = 0x0100010d,

        Key_LaunchG = 0x0100010e,
        Key_LaunchH = 0x0100010f,

        Key_TouchpadToggle = 0x01000110,
        Key_TouchpadOn = 0x01000111,
        Key_TouchpadOff = 0x01000112,

        Key_MicMute = 0x01000113,

        Key_Red = 0x01000114,
        Key_Green = 0x01000115,
        Key_Yellow = 0x01000116,
        Key_Blue = 0x01000117,

        Key_ChannelUp = 0x01000118,
        Key_ChannelDown = 0x01000119,

        Key_Guide = 0x0100011a,
        Key_Info = 0x0100011b,
        Key_Settings = 0x0100011c,

        Key_MicVolumeUp = 0x0100011d,
        Key_MicVolumeDown = 0x0100011e,

        Key_New = 0x01000120,
        Key_Open = 0x01000121,
        Key_Find = 0x01000122,
        Key_Undo = 0x01000123,
        Key_Redo = 0x01000124,

        Key_MediaLast = 0x0100ffff,


        Key_Select = 0x01010000,
        Key_Yes = 0x01010001,
        Key_No = 0x01010002,


        Key_Cancel = 0x01020001,
        Key_Printer = 0x01020002,
        Key_Execute = 0x01020003,
        Key_Sleep = 0x01020004,
        Key_Play = 0x01020005,
        Key_Zoom = 0x01020006,



        Key_Exit = 0x0102000a,


        Key_Context1 = 0x01100000,
        Key_Context2 = 0x01100001,
        Key_Context3 = 0x01100002,
        Key_Context4 = 0x01100003,
        Key_Call = 0x01100004,
        Key_Hangup = 0x01100005,
        Key_Flip = 0x01100006,
        Key_ToggleCallHangup = 0x01100007,
        Key_VoiceDial = 0x01100008,
        Key_LastNumberRedial = 0x01100009,

        Key_Camera = 0x01100020,
        Key_CameraFocus = 0x01100021,

        Key_unknown = 0x01ffffff
    };

    enum ArrowType {
        NoArrow,
        UpArrow,
        DownArrow,
        LeftArrow,
        RightArrow
    };

    enum PenStyle {
        NoPen,
        SolidLine,
        DashLine,
        DotLine,
        DashDotLine,
        DashDotDotLine,
        CustomDashLine

        , MPenStyle = 0x0f

    };

    enum PenCapStyle {
        FlatCap = 0x00,
        SquareCap = 0x10,
        RoundCap = 0x20,
        MPenCapStyle = 0x30
    };

    enum PenJoinStyle {
        MiterJoin = 0x00,
        BevelJoin = 0x40,
        RoundJoin = 0x80,
        SvgMiterJoin = 0x100,
        MPenJoinStyle = 0x1c0
    };

    enum BrushStyle {
        NoBrush,
        SolidPattern,
        Dense1Pattern,
        Dense2Pattern,
        Dense3Pattern,
        Dense4Pattern,
        Dense5Pattern,
        Dense6Pattern,
        Dense7Pattern,
        HorPattern,
        VerPattern,
        CrossPattern,
        BDiagPattern,
        FDiagPattern,
        DiagCrossPattern,
        LinearGradientPattern,
        RadialGradientPattern,
        ConicalGradientPattern,
        TexturePattern = 24
    };

    enum SizeMode {
        AbsoluteSize,
        RelativeSize
    };

    enum UIEffect {
        UI_General,
        UI_AnimateMenu,
        UI_FadeMenu,
        UI_AnimateCombo,
        UI_AnimateTooltip,
        UI_FadeTooltip,
        UI_AnimateToolBox
    };

    enum CursorShape {
        ArrowCursor,
        UpArrowCursor,
        CrossCursor,
        WaitCursor,
        IBeamCursor,
        SizeVerCursor,
        SizeHorCursor,
        SizeBDiagCursor,
        SizeFDiagCursor,
        SizeAllCursor,
        BlankCursor,
        SplitVCursor,
        SplitHCursor,
        PointingHandCursor,
        ForbiddenCursor,
        WhatsThisCursor,
        BusyCursor,
        OpenHandCursor,
        ClosedHandCursor,
        DragCopyCursor,
        DragMoveCursor,
        DragLinkCursor,
        LastCursor = DragLinkCursor,
        BitmapCursor = 24,
        CustomCursor = 25
    };

    enum TextFormat {
        PlainText,
        RichText,
        AutoText
    };

    enum AspectRatioMode {
        IgnoreAspectRatio,
        KeepAspectRatio,
        KeepAspectRatioByExpanding
    };

    enum DockWidgetArea {
        LeftDockWidgetArea = 0x1,
        RightDockWidgetArea = 0x2,
        TopDockWidgetArea = 0x4,
        BottomDockWidgetArea = 0x8,

        DockWidgetArea_Mask = 0xf,
        AllDockWidgetAreas = DockWidgetArea_Mask,
        NoDockWidgetArea = 0
    };
    enum DockWidgetAreaSizes {
        NDockWidgetAreas = 4
    };

    typedef QFlags<DockWidgetArea> DockWidgetAreas;
    constexpr inline QFlags<DockWidgetAreas::enum_type> operator|(DockWidgetAreas::enum_type f1, DockWidgetAreas::enum_type f2) noexcept { return QFlags<DockWidgetAreas::enum_type>(f1) | f2; } constexpr inline QFlags<DockWidgetAreas::enum_type> operator|(DockWidgetAreas::enum_type f1, QFlags<DockWidgetAreas::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(DockWidgetAreas::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum ToolBarArea {
        LeftToolBarArea = 0x1,
        RightToolBarArea = 0x2,
        TopToolBarArea = 0x4,
        BottomToolBarArea = 0x8,

        ToolBarArea_Mask = 0xf,
        AllToolBarAreas = ToolBarArea_Mask,
        NoToolBarArea = 0
    };

    enum ToolBarAreaSizes {
        NToolBarAreas = 4
    };

    typedef QFlags<ToolBarArea> ToolBarAreas;
    constexpr inline QFlags<ToolBarAreas::enum_type> operator|(ToolBarAreas::enum_type f1, ToolBarAreas::enum_type f2) noexcept { return QFlags<ToolBarAreas::enum_type>(f1) | f2; } constexpr inline QFlags<ToolBarAreas::enum_type> operator|(ToolBarAreas::enum_type f1, QFlags<ToolBarAreas::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(ToolBarAreas::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum DateFormat {
        TextDate,
        ISODate,
        SystemLocaleDate,
        LocalDate = SystemLocaleDate,
        LocaleDate,
        SystemLocaleShortDate,
        SystemLocaleLongDate,
        DefaultLocaleShortDate,
        DefaultLocaleLongDate,
        RFC2822Date,
        ISODateWithMs
    };

    enum TimeSpec {
        LocalTime,
        UTC,
        OffsetFromUTC,
        TimeZone
    };

    enum DayOfWeek {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    };

    enum ScrollBarPolicy {
        ScrollBarAsNeeded,
        ScrollBarAlwaysOff,
        ScrollBarAlwaysOn
    };

    enum CaseSensitivity {
        CaseInsensitive,
        CaseSensitive
    };

    enum Corner {
        TopLeftCorner = 0x00000,
        TopRightCorner = 0x00001,
        BottomLeftCorner = 0x00002,
        BottomRightCorner = 0x00003
    };

    enum Edge {
        TopEdge = 0x00001,
        LeftEdge = 0x00002,
        RightEdge = 0x00004,
        BottomEdge = 0x00008
    };

    typedef QFlags<Edge> Edges;
    constexpr inline QFlags<Edges::enum_type> operator|(Edges::enum_type f1, Edges::enum_type f2) noexcept { return QFlags<Edges::enum_type>(f1) | f2; } constexpr inline QFlags<Edges::enum_type> operator|(Edges::enum_type f1, QFlags<Edges::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(Edges::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum ConnectionType {
        AutoConnection,
        DirectConnection,
        QueuedConnection,
        BlockingQueuedConnection,
        UniqueConnection = 0x80
    };

    enum ShortcutContext {
        WidgetShortcut,
        WindowShortcut,
        ApplicationShortcut,
        WidgetWithChildrenShortcut
    };

    enum FillRule {
        OddEvenFill,
        WindingFill
    };

    enum MaskMode {
        MaskInColor,
        MaskOutColor
    };

    enum ClipOperation {
        NoClip,
        ReplaceClip,
        IntersectClip
    };


    enum ItemSelectionMode {
        ContainsItemShape = 0x0,
        IntersectsItemShape = 0x1,
        ContainsItemBoundingRect = 0x2,
        IntersectsItemBoundingRect = 0x3
    };

    enum ItemSelectionOperation {
        ReplaceSelection,
        AddToSelection
    };

    enum TransformationMode {
        FastTransformation,
        SmoothTransformation
    };

    enum Axis {
        XAxis,
        YAxis,
        ZAxis
    };

    enum FocusReason {
        MouseFocusReason,
        TabFocusReason,
        BacktabFocusReason,
        ActiveWindowFocusReason,
        PopupFocusReason,
        ShortcutFocusReason,
        MenuBarFocusReason,
        OtherFocusReason,
        NoFocusReason
    };

    enum ContextMenuPolicy {
        NoContextMenu,
        DefaultContextMenu,
        ActionsContextMenu,
        CustomContextMenu,
        PreventContextMenu
    };

    enum InputMethodQuery {
        ImEnabled = 0x1,
        ImCursorRectangle = 0x2,
        ImMicroFocus = 0x2,
        ImFont = 0x4,
        ImCursorPosition = 0x8,
        ImSurroundingText = 0x10,
        ImCurrentSelection = 0x20,
        ImMaximumTextLength = 0x40,
        ImAnchorPosition = 0x80,
        ImHints = 0x100,
        ImPreferredLanguage = 0x200,

        ImAbsolutePosition = 0x400,
        ImTextBeforeCursor = 0x800,
        ImTextAfterCursor = 0x1000,
        ImEnterKeyType = 0x2000,
        ImAnchorRectangle = 0x4000,
        ImInputItemClipRectangle = 0x8000,

        ImPlatformData = 0x80000000,
        ImQueryInput = ImCursorRectangle | ImCursorPosition | ImSurroundingText |
                       ImCurrentSelection | ImAnchorRectangle | ImAnchorPosition,
        ImQueryAll = 0xffffffff
    };
    typedef QFlags<InputMethodQuery> InputMethodQueries;
    constexpr inline QFlags<InputMethodQueries::enum_type> operator|(InputMethodQueries::enum_type f1, InputMethodQueries::enum_type f2) noexcept { return QFlags<InputMethodQueries::enum_type>(f1) | f2; } constexpr inline QFlags<InputMethodQueries::enum_type> operator|(InputMethodQueries::enum_type f1, QFlags<InputMethodQueries::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(InputMethodQueries::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum InputMethodHint {
        ImhNone = 0x0,

        ImhHiddenText = 0x1,
        ImhSensitiveData = 0x2,
        ImhNoAutoUppercase = 0x4,
        ImhPreferNumbers = 0x8,
        ImhPreferUppercase = 0x10,
        ImhPreferLowercase = 0x20,
        ImhNoPredictiveText = 0x40,

        ImhDate = 0x80,
        ImhTime = 0x100,

        ImhPreferLatin = 0x200,

        ImhMultiLine = 0x400,

        ImhNoEditMenu = 0x800,
        ImhNoTextHandles = 0x1000,

        ImhDigitsOnly = 0x10000,
        ImhFormattedNumbersOnly = 0x20000,
        ImhUppercaseOnly = 0x40000,
        ImhLowercaseOnly = 0x80000,
        ImhDialableCharactersOnly = 0x100000,
        ImhEmailCharactersOnly = 0x200000,
        ImhUrlCharactersOnly = 0x400000,
        ImhLatinOnly = 0x800000,

        ImhExclusiveInputMask = 0xffff0000
    };
    typedef QFlags<InputMethodHint> InputMethodHints;
    constexpr inline QFlags<InputMethodHints::enum_type> operator|(InputMethodHints::enum_type f1, InputMethodHints::enum_type f2) noexcept { return QFlags<InputMethodHints::enum_type>(f1) | f2; } constexpr inline QFlags<InputMethodHints::enum_type> operator|(InputMethodHints::enum_type f1, QFlags<InputMethodHints::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(InputMethodHints::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum EnterKeyType {
        EnterKeyDefault,
        EnterKeyReturn,
        EnterKeyDone,
        EnterKeyGo,
        EnterKeySend,
        EnterKeySearch,
        EnterKeyNext,
        EnterKeyPrevious
    };

    enum ToolButtonStyle {
        ToolButtonIconOnly,
        ToolButtonTextOnly,
        ToolButtonTextBesideIcon,
        ToolButtonTextUnderIcon,
        ToolButtonFollowStyle
    };

    enum LayoutDirection {
        LeftToRight,
        RightToLeft,
        LayoutDirectionAuto
    };

    enum AnchorPoint {
        AnchorLeft = 0,
        AnchorHorizontalCenter,
        AnchorRight,
        AnchorTop,
        AnchorVerticalCenter,
        AnchorBottom
    };

    enum FindChildOption {
        FindDirectChildrenOnly = 0x0,
        FindChildrenRecursively = 0x1
    };
    typedef QFlags<FindChildOption> FindChildOptions;

    enum DropAction {
        CopyAction = 0x1,
        MoveAction = 0x2,
        LinkAction = 0x4,
        ActionMask = 0xff,
        TargetMoveAction = 0x8002,
        IgnoreAction = 0x0
    };
    typedef QFlags<DropAction> DropActions;
    constexpr inline QFlags<DropActions::enum_type> operator|(DropActions::enum_type f1, DropActions::enum_type f2) noexcept { return QFlags<DropActions::enum_type>(f1) | f2; } constexpr inline QFlags<DropActions::enum_type> operator|(DropActions::enum_type f1, QFlags<DropActions::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(DropActions::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum CheckState {
        Unchecked,
        PartiallyChecked,
        Checked
    };

    enum ItemDataRole {
        DisplayRole = 0,
        DecorationRole = 1,
        EditRole = 2,
        ToolTipRole = 3,
        StatusTipRole = 4,
        WhatsThisRole = 5,

        FontRole = 6,
        TextAlignmentRole = 7,
        BackgroundColorRole = 8,
        BackgroundRole = 8,
        TextColorRole = 9,
        ForegroundRole = 9,
        CheckStateRole = 10,

        AccessibleTextRole = 11,
        AccessibleDescriptionRole = 12,

        SizeHintRole = 13,
        InitialSortOrderRole = 14,

        DisplayPropertyRole = 27,
        DecorationPropertyRole = 28,
        ToolTipPropertyRole = 29,
        StatusTipPropertyRole = 30,
        WhatsThisPropertyRole = 31,

        UserRole = 0x0100
    };

    enum ItemFlag {
        NoItemFlags = 0,
        ItemIsSelectable = 1,
        ItemIsEditable = 2,
        ItemIsDragEnabled = 4,
        ItemIsDropEnabled = 8,
        ItemIsUserCheckable = 16,
        ItemIsEnabled = 32,
        ItemIsAutoTristate = 64,

        ItemIsTristate = ItemIsAutoTristate,

        ItemNeverHasChildren = 128,
        ItemIsUserTristate = 256
    };
    typedef QFlags<ItemFlag> ItemFlags;
    constexpr inline QFlags<ItemFlags::enum_type> operator|(ItemFlags::enum_type f1, ItemFlags::enum_type f2) noexcept { return QFlags<ItemFlags::enum_type>(f1) | f2; } constexpr inline QFlags<ItemFlags::enum_type> operator|(ItemFlags::enum_type f1, QFlags<ItemFlags::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(ItemFlags::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum MatchFlag {
        MatchExactly = 0,
        MatchContains = 1,
        MatchStartsWith = 2,
        MatchEndsWith = 3,
        MatchRegExp = 4,
        MatchWildcard = 5,
        MatchFixedString = 8,
        MatchCaseSensitive = 16,
        MatchWrap = 32,
        MatchRecursive = 64
    };
    typedef QFlags<MatchFlag> MatchFlags;
    constexpr inline QFlags<MatchFlags::enum_type> operator|(MatchFlags::enum_type f1, MatchFlags::enum_type f2) noexcept { return QFlags<MatchFlags::enum_type>(f1) | f2; } constexpr inline QFlags<MatchFlags::enum_type> operator|(MatchFlags::enum_type f1, QFlags<MatchFlags::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(MatchFlags::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    typedef void * HANDLE;




    enum WindowModality {
        NonModal,
        WindowModal,
        ApplicationModal
    };

    enum TextInteractionFlag {
        NoTextInteraction = 0,
        TextSelectableByMouse = 1,
        TextSelectableByKeyboard = 2,
        LinksAccessibleByMouse = 4,
        LinksAccessibleByKeyboard = 8,
        TextEditable = 16,

        TextEditorInteraction = TextSelectableByMouse | TextSelectableByKeyboard | TextEditable,
        TextBrowserInteraction = TextSelectableByMouse | LinksAccessibleByMouse | LinksAccessibleByKeyboard
    };
    typedef QFlags<TextInteractionFlag> TextInteractionFlags;
    constexpr inline QFlags<TextInteractionFlags::enum_type> operator|(TextInteractionFlags::enum_type f1, TextInteractionFlags::enum_type f2) noexcept { return QFlags<TextInteractionFlags::enum_type>(f1) | f2; } constexpr inline QFlags<TextInteractionFlags::enum_type> operator|(TextInteractionFlags::enum_type f1, QFlags<TextInteractionFlags::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(TextInteractionFlags::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum EventPriority {
        HighEventPriority = 1,
        NormalEventPriority = 0,
        LowEventPriority = -1
    };

    enum SizeHint {
        MinimumSize,
        PreferredSize,
        MaximumSize,
        MinimumDescent,
        NSizeHints
    };

    enum WindowFrameSection {
        NoSection,
        LeftSection,
        TopLeftSection,
        TopSection,
        TopRightSection,
        RightSection,
        BottomRightSection,
        BottomSection,
        BottomLeftSection,
        TitleBarArea
    };


    enum class Initialization {
        Uninitialized
    };
    static constexpr __attribute__((__unused__)) Initialization Uninitialized = Initialization::Uninitialized;






    enum CoordinateSystem {
        DeviceCoordinates,
        LogicalCoordinates
    };

    enum TouchPointState {
        TouchPointPressed = 0x01,
        TouchPointMoved = 0x02,
        TouchPointStationary = 0x04,
        TouchPointReleased = 0x08
    };
    typedef QFlags<TouchPointState> TouchPointStates;
    constexpr inline QFlags<TouchPointStates::enum_type> operator|(TouchPointStates::enum_type f1, TouchPointStates::enum_type f2) noexcept { return QFlags<TouchPointStates::enum_type>(f1) | f2; } constexpr inline QFlags<TouchPointStates::enum_type> operator|(TouchPointStates::enum_type f1, QFlags<TouchPointStates::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(TouchPointStates::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }


    enum GestureState
    {
        NoGesture,
        GestureStarted = 1,
        GestureUpdated = 2,
        GestureFinished = 3,
        GestureCanceled = 4
    };

    enum GestureType
    {
        TapGesture = 1,
        TapAndHoldGesture = 2,
        PanGesture = 3,
        PinchGesture = 4,
        SwipeGesture = 5,

        CustomGesture = 0x0100,

        LastGestureType = ~0u
    };

    enum GestureFlag
    {
        DontStartGestureOnChildren = 0x01,
        ReceivePartialGestures = 0x02,
        IgnoredGesturesPropagateToParent = 0x04
    };
    typedef QFlags<GestureFlag> GestureFlags;
    constexpr inline QFlags<GestureFlags::enum_type> operator|(GestureFlags::enum_type f1, GestureFlags::enum_type f2) noexcept { return QFlags<GestureFlags::enum_type>(f1) | f2; } constexpr inline QFlags<GestureFlags::enum_type> operator|(GestureFlags::enum_type f1, QFlags<GestureFlags::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(GestureFlags::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum NativeGestureType
    {
        BeginNativeGesture,
        EndNativeGesture,
        PanNativeGesture,
        ZoomNativeGesture,
        SmartZoomNativeGesture,
        RotateNativeGesture,
        SwipeNativeGesture
    };



    enum NavigationMode
    {
        NavigationModeNone,
        NavigationModeKeypadTabOrder,
        NavigationModeKeypadDirectional,
        NavigationModeCursorAuto,
        NavigationModeCursorForceVisible
    };

    enum CursorMoveStyle {
        LogicalMoveStyle,
        VisualMoveStyle
    };

    enum TimerType {
        PreciseTimer,
        CoarseTimer,
        VeryCoarseTimer
    };

    enum ScrollPhase {
        NoScrollPhase = 0,
        ScrollBegin,
        ScrollUpdate,
        ScrollEnd,
        ScrollMomentum
    };

    enum MouseEventSource {
        MouseEventNotSynthesized,
        MouseEventSynthesizedBySystem,
        MouseEventSynthesizedByQt,
        MouseEventSynthesizedByApplication
    };

    enum MouseEventFlag {
        MouseEventCreatedDoubleClick = 0x01,
        MouseEventFlagMask = 0xFF
    };
    typedef QFlags<MouseEventFlag> MouseEventFlags;
    constexpr inline QFlags<MouseEventFlags::enum_type> operator|(MouseEventFlags::enum_type f1, MouseEventFlags::enum_type f2) noexcept { return QFlags<MouseEventFlags::enum_type>(f1) | f2; } constexpr inline QFlags<MouseEventFlags::enum_type> operator|(MouseEventFlags::enum_type f1, QFlags<MouseEventFlags::enum_type> f2) noexcept { return f2 | f1; } constexpr inline QIncompatibleFlag operator|(MouseEventFlags::enum_type f1, int f2) noexcept { return QIncompatibleFlag(int(f1) | f2); }

    enum ChecksumType {
        ChecksumIso3309,
        ChecksumItuV41
    };



    inline const QMetaObject *qt_getEnumMetaObject(ScrollBarPolicy) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ScrollBarPolicy) noexcept { return "ScrollBarPolicy"; }
    inline const QMetaObject *qt_getEnumMetaObject(FocusPolicy) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(FocusPolicy) noexcept { return "FocusPolicy"; }
    inline const QMetaObject *qt_getEnumMetaObject(ContextMenuPolicy) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ContextMenuPolicy) noexcept { return "ContextMenuPolicy"; }
    inline const QMetaObject *qt_getEnumMetaObject(ArrowType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ArrowType) noexcept { return "ArrowType"; }
    inline const QMetaObject *qt_getEnumMetaObject(ToolButtonStyle) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ToolButtonStyle) noexcept { return "ToolButtonStyle"; }
    inline const QMetaObject *qt_getEnumMetaObject(PenStyle) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(PenStyle) noexcept { return "PenStyle"; }
    inline const QMetaObject *qt_getEnumMetaObject(PenCapStyle) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(PenCapStyle) noexcept { return "PenCapStyle"; }
    inline const QMetaObject *qt_getEnumMetaObject(PenJoinStyle) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(PenJoinStyle) noexcept { return "PenJoinStyle"; }
    inline const QMetaObject *qt_getEnumMetaObject(BrushStyle) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(BrushStyle) noexcept { return "BrushStyle"; }
    inline const QMetaObject *qt_getEnumMetaObject(FillRule) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(FillRule) noexcept { return "FillRule"; }
    inline const QMetaObject *qt_getEnumMetaObject(MaskMode) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(MaskMode) noexcept { return "MaskMode"; }
    inline const QMetaObject *qt_getEnumMetaObject(BGMode) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(BGMode) noexcept { return "BGMode"; }
    inline const QMetaObject *qt_getEnumMetaObject(ClipOperation) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ClipOperation) noexcept { return "ClipOperation"; }
    inline const QMetaObject *qt_getEnumMetaObject(SizeMode) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(SizeMode) noexcept { return "SizeMode"; }
    inline const QMetaObject *qt_getEnumMetaObject(Axis) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Axis) noexcept { return "Axis"; }
    inline const QMetaObject *qt_getEnumMetaObject(Corner) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Corner) noexcept { return "Corner"; }
    inline const QMetaObject *qt_getEnumMetaObject(Edge) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Edge) noexcept { return "Edge"; }
    inline const QMetaObject *qt_getEnumMetaObject(LayoutDirection) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(LayoutDirection) noexcept { return "LayoutDirection"; }
    inline const QMetaObject *qt_getEnumMetaObject(SizeHint) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(SizeHint) noexcept { return "SizeHint"; }
    inline const QMetaObject *qt_getEnumMetaObject(Orientation) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Orientation) noexcept { return "Orientation"; }
    inline const QMetaObject *qt_getEnumMetaObject(DropAction) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(DropAction) noexcept { return "DropAction"; }
    inline const QMetaObject *qt_getEnumMetaObject(Alignment) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Alignment) noexcept { return "Alignment"; }
    inline const QMetaObject *qt_getEnumMetaObject(TextFlag) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TextFlag) noexcept { return "TextFlag"; }
    inline const QMetaObject *qt_getEnumMetaObject(Orientations) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Orientations) noexcept { return "Orientations"; }
    inline const QMetaObject *qt_getEnumMetaObject(DropActions) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(DropActions) noexcept { return "DropActions"; }
    inline const QMetaObject *qt_getEnumMetaObject(Edges) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Edges) noexcept { return "Edges"; }
    inline const QMetaObject *qt_getEnumMetaObject(DockWidgetAreas) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(DockWidgetAreas) noexcept { return "DockWidgetAreas"; }
    inline const QMetaObject *qt_getEnumMetaObject(ToolBarAreas) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ToolBarAreas) noexcept { return "ToolBarAreas"; }
    inline const QMetaObject *qt_getEnumMetaObject(DockWidgetArea) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(DockWidgetArea) noexcept { return "DockWidgetArea"; }
    inline const QMetaObject *qt_getEnumMetaObject(ToolBarArea) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ToolBarArea) noexcept { return "ToolBarArea"; }
    inline const QMetaObject *qt_getEnumMetaObject(TextFormat) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TextFormat) noexcept { return "TextFormat"; }
    inline const QMetaObject *qt_getEnumMetaObject(TextElideMode) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TextElideMode) noexcept { return "TextElideMode"; }
    inline const QMetaObject *qt_getEnumMetaObject(DateFormat) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(DateFormat) noexcept { return "DateFormat"; }
    inline const QMetaObject *qt_getEnumMetaObject(TimeSpec) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TimeSpec) noexcept { return "TimeSpec"; }
    inline const QMetaObject *qt_getEnumMetaObject(DayOfWeek) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(DayOfWeek) noexcept { return "DayOfWeek"; }
    inline const QMetaObject *qt_getEnumMetaObject(CursorShape) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(CursorShape) noexcept { return "CursorShape"; }
    inline const QMetaObject *qt_getEnumMetaObject(GlobalColor) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(GlobalColor) noexcept { return "GlobalColor"; }
    inline const QMetaObject *qt_getEnumMetaObject(AspectRatioMode) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(AspectRatioMode) noexcept { return "AspectRatioMode"; }
    inline const QMetaObject *qt_getEnumMetaObject(TransformationMode) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TransformationMode) noexcept { return "TransformationMode"; }
    inline const QMetaObject *qt_getEnumMetaObject(ImageConversionFlags) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ImageConversionFlags) noexcept { return "ImageConversionFlags"; }
    inline const QMetaObject *qt_getEnumMetaObject(Key) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(Key) noexcept { return "Key"; }
    inline const QMetaObject *qt_getEnumMetaObject(ShortcutContext) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ShortcutContext) noexcept { return "ShortcutContext"; }
    inline const QMetaObject *qt_getEnumMetaObject(TextInteractionFlag) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TextInteractionFlag) noexcept { return "TextInteractionFlag"; }
    inline const QMetaObject *qt_getEnumMetaObject(TextInteractionFlags) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TextInteractionFlags) noexcept { return "TextInteractionFlags"; }
    inline const QMetaObject *qt_getEnumMetaObject(ItemSelectionMode) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ItemSelectionMode) noexcept { return "ItemSelectionMode"; }
    inline const QMetaObject *qt_getEnumMetaObject(ItemSelectionOperation) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ItemSelectionOperation) noexcept { return "ItemSelectionOperation"; }
    inline const QMetaObject *qt_getEnumMetaObject(ItemFlags) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ItemFlags) noexcept { return "ItemFlags"; }
    inline const QMetaObject *qt_getEnumMetaObject(CheckState) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(CheckState) noexcept { return "CheckState"; }
    inline const QMetaObject *qt_getEnumMetaObject(ItemDataRole) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ItemDataRole) noexcept { return "ItemDataRole"; }
    inline const QMetaObject *qt_getEnumMetaObject(SortOrder) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(SortOrder) noexcept { return "SortOrder"; }
    inline const QMetaObject *qt_getEnumMetaObject(CaseSensitivity) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(CaseSensitivity) noexcept { return "CaseSensitivity"; }
    inline const QMetaObject *qt_getEnumMetaObject(MatchFlags) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(MatchFlags) noexcept { return "MatchFlags"; }
    inline const QMetaObject *qt_getEnumMetaObject(KeyboardModifiers) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(KeyboardModifiers) noexcept { return "KeyboardModifiers"; }
    inline const QMetaObject *qt_getEnumMetaObject(MouseButtons) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(MouseButtons) noexcept { return "MouseButtons"; }
    inline const QMetaObject *qt_getEnumMetaObject(WindowType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(WindowType) noexcept { return "WindowType"; }
    inline const QMetaObject *qt_getEnumMetaObject(WindowState) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(WindowState) noexcept { return "WindowState"; }
    inline const QMetaObject *qt_getEnumMetaObject(WindowModality) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(WindowModality) noexcept { return "WindowModality"; }
    inline const QMetaObject *qt_getEnumMetaObject(WidgetAttribute) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(WidgetAttribute) noexcept { return "WidgetAttribute"; }
    inline const QMetaObject *qt_getEnumMetaObject(ApplicationAttribute) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ApplicationAttribute) noexcept { return "ApplicationAttribute"; }
    inline const QMetaObject *qt_getEnumMetaObject(WindowFlags) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(WindowFlags) noexcept { return "WindowFlags"; }
    inline const QMetaObject *qt_getEnumMetaObject(WindowStates) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(WindowStates) noexcept { return "WindowStates"; }
    inline const QMetaObject *qt_getEnumMetaObject(FocusReason) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(FocusReason) noexcept { return "FocusReason"; }
    inline const QMetaObject *qt_getEnumMetaObject(InputMethodHint) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(InputMethodHint) noexcept { return "InputMethodHint"; }
    inline const QMetaObject *qt_getEnumMetaObject(InputMethodQuery) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(InputMethodQuery) noexcept { return "InputMethodQuery"; }
    inline const QMetaObject *qt_getEnumMetaObject(InputMethodHints) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(InputMethodHints) noexcept { return "InputMethodHints"; }
    inline const QMetaObject *qt_getEnumMetaObject(EnterKeyType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(EnterKeyType) noexcept { return "EnterKeyType"; }
    inline const QMetaObject *qt_getEnumMetaObject(InputMethodQueries) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(InputMethodQueries) noexcept { return "InputMethodQueries"; }
    inline const QMetaObject *qt_getEnumMetaObject(TouchPointStates) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TouchPointStates) noexcept { return "TouchPointStates"; }
    inline const QMetaObject *qt_getEnumMetaObject(ScreenOrientation) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ScreenOrientation) noexcept { return "ScreenOrientation"; }
    inline const QMetaObject *qt_getEnumMetaObject(ScreenOrientations) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ScreenOrientations) noexcept { return "ScreenOrientations"; }
    inline const QMetaObject *qt_getEnumMetaObject(ConnectionType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ConnectionType) noexcept { return "ConnectionType"; }
    inline const QMetaObject *qt_getEnumMetaObject(ApplicationState) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ApplicationState) noexcept { return "ApplicationState"; }

    inline const QMetaObject *qt_getEnumMetaObject(GestureState) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(GestureState) noexcept { return "GestureState"; }
    inline const QMetaObject *qt_getEnumMetaObject(GestureType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(GestureType) noexcept { return "GestureType"; }
    inline const QMetaObject *qt_getEnumMetaObject(NativeGestureType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(NativeGestureType) noexcept { return "NativeGestureType"; }

    inline const QMetaObject *qt_getEnumMetaObject(CursorMoveStyle) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(CursorMoveStyle) noexcept { return "CursorMoveStyle"; }
    inline const QMetaObject *qt_getEnumMetaObject(TimerType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TimerType) noexcept { return "TimerType"; }
    inline const QMetaObject *qt_getEnumMetaObject(ScrollPhase) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ScrollPhase) noexcept { return "ScrollPhase"; }
    inline const QMetaObject *qt_getEnumMetaObject(MouseEventSource) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(MouseEventSource) noexcept { return "MouseEventSource"; }
    inline const QMetaObject *qt_getEnumMetaObject(MouseEventFlag) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(MouseEventFlag) noexcept { return "MouseEventFlag"; }
    inline const QMetaObject *qt_getEnumMetaObject(ChecksumType) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(ChecksumType) noexcept { return "ChecksumType"; }
    inline const QMetaObject *qt_getEnumMetaObject(TabFocusBehavior) noexcept { return qt_getQtMetaObject(); } inline constexpr const char *qt_getEnumName(TabFocusBehavior) noexcept { return "TabFocusBehavior"; }


}







typedef bool (*qInternalCallback)(void **);

class __attribute__((visibility("default"))) QInternal {
public:
    enum PaintDeviceFlags {
        UnknownDevice = 0x00,
        Widget = 0x01,
        Pixmap = 0x02,
        Image = 0x03,
        Printer = 0x04,
        Picture = 0x05,
        Pbuffer = 0x06,
        FramebufferObject = 0x07,
        CustomRaster = 0x08,
        MacQuartz = 0x09,
        PaintBuffer = 0x0a,
        OpenGL = 0x0b
    };
    enum RelayoutType {
        RelayoutNormal,
        RelayoutDragging,
        RelayoutDropped
    };

    enum DockPosition {
        LeftDock,
        RightDock,
        TopDock,
        BottomDock,
        DockCount
    };

    enum Callback {
        EventNotifyCallback,
        LastCallback
    };
    static bool registerCallback(Callback, qInternalCallback);
    static bool unregisterCallback(Callback, qInternalCallback);
    static bool activateCallbacks(Callback, void **);
};


