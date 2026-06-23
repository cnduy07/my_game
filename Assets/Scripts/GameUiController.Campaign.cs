using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class GameUiController
{
    void BuildLevelSelectOverlay(Transform parent)
    {
        levelSelectOverlay = new GameObject("LevelSelectOverlay", typeof(RectTransform), typeof(Image));
        levelSelectOverlay.transform.SetParent(parent, false);
        Image overlayImage = levelSelectOverlay.GetComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.64f);
        SetAnchor((RectTransform)levelSelectOverlay.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        RectTransform card = CreatePanel("LevelSelectCard", levelSelectOverlay.transform, new Color(0.06f, 0.075f, 0.1f, 0.98f));
        AddFrame(card, new Color(0.14f, 0.26f, 0.34f, 0.95f), new Vector2(2f, -2f));
        SetAnchor(card, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-780f, -420f), new Vector2(780f, 420f));

        TextMeshProUGUI title = CreateText("Title", card, "CAMPAIGN MAP", 34, FontStyle.Bold, TextAnchor.MiddleCenter);
        SetAnchor(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(30f, -76f), new Vector2(-30f, -22f));

        TextMeshProUGUI subtitle = CreateText("Subtitle", card, "Select a sector, review enemy intel, then deploy.", 18, FontStyle.Bold, TextAnchor.MiddleCenter);
        subtitle.color = new Color(0.8f, 0.9f, 0.96f, 1f);
        SetAnchor(subtitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(42f, -112f), new Vector2(-42f, -78f));

        campaignMapPanel = CreatePanel("CampaignMap", card, new Color(0.025f, 0.035f, 0.052f, 0.94f));
        AddFrame(campaignMapPanel, new Color(0.08f, 0.16f, 0.22f, 0.9f));
        SetAnchor(campaignMapPanel, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(46f, 104f), new Vector2(-448f, -132f));
        campaignMapPanel.gameObject.AddComponent<RectMask2D>();

        campaignMapScroll = campaignMapPanel.gameObject.AddComponent<ScrollRect>();
        campaignMapScroll.horizontal = true;
        campaignMapScroll.vertical = false;
        campaignMapScroll.movementType = ScrollRect.MovementType.Clamped;
        campaignMapScroll.scrollSensitivity = 42f;
        campaignMapScroll.inertia = true;
        campaignMapScroll.decelerationRate = 0.135f;
        campaignMapScroll.viewport = campaignMapPanel;

        campaignMapContent = new GameObject("CampaignMapContent", typeof(RectTransform)).GetComponent<RectTransform>();
        campaignMapContent.SetParent(campaignMapPanel, false);
        campaignMapContent.pivot = new Vector2(0f, 0.5f);
        SetAnchor(campaignMapContent, new Vector2(0f, 0f), new Vector2(0f, 1f), Vector2.zero, Vector2.zero);
        campaignMapScroll.content = campaignMapContent;

        campaignRouteLayer = new GameObject("RouteLayer", typeof(RectTransform)).GetComponent<RectTransform>();
        campaignRouteLayer.SetParent(campaignMapContent, false);
        SetAnchor(campaignRouteLayer, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        campaignNodeLayer = new GameObject("NodeLayer", typeof(RectTransform)).GetComponent<RectTransform>();
        campaignNodeLayer.SetParent(campaignMapContent, false);
        SetAnchor(campaignNodeLayer, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        BuildMissionDetailPanel(card);

        Button closeButton = CreateButton("CloseButton", card, "CLOSE", 20, panelSoftColor, Color.white);
        closeButton.onClick.AddListener(CloseLevelSelect);
        SetAnchor((RectTransform)closeButton.transform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-398f, 30f), new Vector2(-210f, 82f));

        levelSelectOverlay.SetActive(false);
    }

    void BuildMissionDetailPanel(RectTransform card)
    {
        missionDetailPanel = CreatePanel("MissionDetail", card, new Color(0.035f, 0.048f, 0.07f, 0.96f));
        AddFrame(missionDetailPanel, new Color(0.1f, 0.2f, 0.27f, 0.9f));
        SetAnchor(missionDetailPanel, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-420f, 104f), new Vector2(-46f, -132f));

        missionStatusText = CreateText("Status", missionDetailPanel, "", 17, FontStyle.Bold, TextAnchor.MiddleLeft);
        missionStatusText.color = accentColor;
        SetAnchor(missionStatusText.rectTransform, new Vector2(0f, 1f), new Vector2(0.54f, 1f), new Vector2(24f, -44f), new Vector2(-8f, -12f));

        missionDevModeText = CreateText("DevMode", missionDetailPanel, "", 13, FontStyle.Bold, TextAnchor.MiddleRight);
        missionDevModeText.color = warningColor;
        SetAnchor(missionDevModeText.rectTransform, new Vector2(0.54f, 1f), new Vector2(1f, 1f), new Vector2(8f, -44f), new Vector2(-24f, -12f));

        missionTitleText = CreateText("Title", missionDetailPanel, "", 28, FontStyle.Bold, TextAnchor.MiddleLeft);
        SetAnchor(missionTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -92f), new Vector2(-24f, -42f));

        missionTypeText = CreateText("Type", missionDetailPanel, "", 18, FontStyle.Bold, TextAnchor.MiddleLeft);
        missionTypeText.color = new Color(0.82f, 0.93f, 0.98f, 1f);
        SetAnchor(missionTypeText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -126f), new Vector2(-24f, -92f));

        missionBriefingText = CreateText("Briefing", missionDetailPanel, "", 17, FontStyle.Normal, TextAnchor.UpperLeft);
        missionBriefingText.color = new Color(0.82f, 0.9f, 0.94f, 1f);
        SetAnchor(missionBriefingText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -236f), new Vector2(-24f, -136f));

        missionEnemyMixText = CreateText("EnemyMix", missionDetailPanel, "", 17, FontStyle.Bold, TextAnchor.UpperLeft);
        missionEnemyMixText.color = Color.white;
        SetAnchor(missionEnemyMixText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -320f), new Vector2(-24f, -248f));

        missionToolsText = CreateText("Tools", missionDetailPanel, "", 17, FontStyle.Bold, TextAnchor.UpperLeft);
        missionToolsText.color = new Color(0.78f, 0.95f, 1f, 1f);
        SetAnchor(missionToolsText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(24f, -398f), new Vector2(-24f, -324f));

        missionPressureText = CreateText("Pressure", missionDetailPanel, "", 16, FontStyle.Bold, TextAnchor.MiddleLeft);
        missionPressureText.color = warningColor;
        SetAnchor(missionPressureText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(24f, 154f), new Vector2(-24f, 190f));

        missionRewardText = CreateText("Reward", missionDetailPanel, "", 16, FontStyle.Normal, TextAnchor.UpperLeft);
        missionRewardText.color = new Color(0.78f, 0.88f, 0.94f, 1f);
        SetAnchor(missionRewardText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(24f, 96f), new Vector2(-24f, 154f));

        missionDeployButton = CreateButton("DeployButton", missionDetailPanel, "DEPLOY", 22, accentColor, Color.white);
        missionDeployButton.onClick.AddListener(DeploySelectedMission);
        missionDeployButtonText = missionDeployButton.GetComponentInChildren<TextMeshProUGUI>();
        AddFrame((RectTransform)missionDeployButton.transform, new Color(0.08f, 0.55f, 0.65f, 0.9f));
        SetAnchor((RectTransform)missionDeployButton.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(24f, 26f), new Vector2(-24f, 78f));
    }

    void RebuildLevelButtons(int levelCount)
    {
        foreach (CampaignNode levelButton in levelButtons)
            if (levelButton.button != null) Destroy(levelButton.button.gameObject);
        levelButtons.Clear();
        lastLevelCount = levelCount;

        if (campaignMapPanel == null || campaignMapContent == null || campaignRouteLayer == null || campaignNodeLayer == null) return;

        foreach (Transform child in campaignRouteLayer)
            Destroy(child.gameObject);

        float contentWidth = CampaignMapContentWidth(levelCount);
        SetCampaignMapContentWidth(contentWidth);

        LevelManager levelManager = LevelManager.Instance;
        BuildCampaignRoute(levelCount);

        for (int i = 0; i < levelCount; i++)
        {
            LevelDefinition level = levelManager != null ? levelManager.GetLevelAt(i) : null;
            if (level == null) continue;

            LevelDefinition capturedLevel = level;
            GameObject go = new GameObject($"MissionNode_{i}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(campaignNodeLayer, false);

            RectTransform rect = (RectTransform)go.transform;
            bool finale = i == levelCount - 1;
            float width = finale ? CampaignNodeWidth + 24f : CampaignNodeWidth;
            float height = finale ? CampaignNodeHeight + 12f : CampaignNodeHeight;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = CampaignNodePosition(i);

            Image frame = go.GetComponent<Image>();
            frame.color = panelSoftColor;
            AddFrame(rect, new Color(0.1f, 0.2f, 0.27f, 0.85f));

            Button button = go.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.colors = BuildButtonColors(panelSoftColor, accentColor);
            button.onClick.AddListener(() => SelectCampaignLevel(capturedLevel));

            TextMeshProUGUI label = CreateText("Number", go.transform, "", finale ? 25 : 22, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetAnchor(label.rectTransform, new Vector2(0f, 0.42f), new Vector2(1f, 1f), new Vector2(8f, -2f), new Vector2(-8f, -2f));

            TextMeshProUGUI type = CreateText("Type", go.transform, "", 12, FontStyle.Bold, TextAnchor.MiddleCenter);
            type.color = new Color(0.82f, 0.93f, 0.98f, 1f);
            SetAnchor(type.rectTransform, new Vector2(0f, 0.16f), new Vector2(1f, 0.5f), new Vector2(6f, 0f), new Vector2(-6f, 0f));

            TextMeshProUGUI status = CreateText("Status", go.transform, "", 11, FontStyle.Bold, TextAnchor.MiddleCenter);
            SetAnchor(status.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.22f), new Vector2(6f, 0f), new Vector2(-6f, 1f));

            levelButtons.Add(new CampaignNode
            {
                button = button,
                frame = frame,
                rect = rect,
                label = label,
                type = type,
                status = status,
                level = level
            });
        }

        if (selectedCampaignLevel == null && levelManager != null)
            selectedCampaignLevel = levelManager.currentLevel;
    }

    void RefreshLevelButtons()
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null) return;

        for (int i = 0; i < levelButtons.Count; i++)
        {
            LevelDefinition level = levelButtons[i].level != null ? levelButtons[i].level : levelManager.GetLevelAt(i);
            if (level == null) continue;

            CampaignNode button = levelButtons[i];
            bool current = level == levelManager.currentLevel;
            bool completed = PlayerProgress.IsLevelCompleted(level);
            bool unlocked = levelManager.IsLevelUnlocked(level);
            bool selected = level == selectedCampaignLevel;
            MissionNodeType nodeType = CampaignIntel.NodeTypeFor(level);

            button.label.text = level.levelNumber.ToString("00");
            button.type.text = CampaignIntel.NodeTypeLabel(nodeType);
            button.status.text = BuildLevelStatus(level, current, completed, unlocked);

            if (selected)
                button.frame.color = accentColor;
            else if (current)
                button.frame.color = new Color(0.18f, 0.42f, 0.52f, 0.96f);
            else
                button.frame.color = unlocked ? ColorForNode(nodeType) : disabledColor;

            button.button.interactable = unlocked;
        }

        RefreshCampaignDetail();
    }

    string BuildLevelDetail(LevelDefinition level, bool unlocked, bool completed)
    {
        if (!unlocked)
            return "Complete the previous mission to unlock.";

        if (completed && !string.IsNullOrWhiteSpace(level.completionReward))
            return level.completionReward;

        if (!string.IsNullOrWhiteSpace(level.missionBriefing))
            return level.missionBriefing;

        return completed ? "Mission cleared." : "Ready for deployment.";
    }

    string BuildLevelStatus(LevelDefinition level, bool current, bool completed, bool unlocked)
    {
        if (!unlocked) return "LOCKED";
        if (current && completed) return "ACTIVE + CLEAR";
        if (current) return "ACTIVE";
        return completed ? "CLEAR" : "READY";
    }

    void BuildCampaignRoute(int levelCount)
    {
        if (campaignRouteLayer == null || campaignMapContent == null) return;

        int routeCount = levelCount - 1;
        for (int i = 0; i < routeCount; i++)
        {
            Vector2 from = CampaignNodePosition(i);
            Vector2 to = CampaignNodePosition(i + 1);
            Vector2 pivot = new Vector2(to.x, from.y);
            CreateRouteLine($"RouteH_{i}", from, pivot);
            CreateRouteLine($"RouteV_{i}", pivot, to);
        }
    }

    void CreateRouteLine(string name, Vector2 from, Vector2 to)
    {
        if (campaignRouteLayer == null) return;
        if (Vector2.Distance(from, to) < 0.01f) return;

        Image line = CreateImage(name, campaignRouteLayer, new Color(0.09f, 0.55f, 0.66f, 0.62f));
        RectTransform rect = line.rectTransform;
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        if (Mathf.Abs(from.y - to.y) <= Mathf.Abs(from.x - to.x))
        {
            float xMin = Mathf.Min(from.x, to.x);
            float xMax = Mathf.Max(from.x, to.x);
            float y = from.y;
            rect.anchoredPosition = new Vector2((xMin + xMax) * 0.5f, y);
            rect.sizeDelta = new Vector2(Mathf.Max(6f, xMax - xMin), 6f);
        }
        else
        {
            float x = from.x;
            float yMin = Mathf.Min(from.y, to.y);
            float yMax = Mathf.Max(from.y, to.y);
            rect.anchoredPosition = new Vector2(x, (yMin + yMax) * 0.5f);
            rect.sizeDelta = new Vector2(6f, Mathf.Max(6f, yMax - yMin));
        }
    }

    float CampaignMapContentWidth(int levelCount)
    {
        if (levelCount <= 0) return CampaignMapMinContentWidth;
        return Mathf.Max(CampaignMapMinContentWidth, CampaignMapSidePadding * 2f + (levelCount - 1) * CampaignNodeStep);
    }

    void SetCampaignMapContentWidth(float width)
    {
        if (campaignMapContent == null) return;

        campaignMapContent.anchorMin = new Vector2(0f, 0f);
        campaignMapContent.anchorMax = new Vector2(0f, 1f);
        campaignMapContent.offsetMin = Vector2.zero;
        campaignMapContent.offsetMax = new Vector2(width, 0f);
    }

    Vector2 CampaignNodePosition(int index)
    {
        return new Vector2(CampaignMapSidePadding + index * CampaignNodeStep, CampaignNodeOffsetY(index));
    }

    float CampaignNodeOffsetY(int index)
    {
        switch (index % 10)
        {
            case 1: return 94f;
            case 2: return -26f;
            case 3: return 138f;
            case 4: return 22f;
            case 5: return -126f;
            case 6: return 104f;
            case 7: return -58f;
            case 8: return 150f;
            case 9: return -102f;
            default: return -92f;
        }
    }

    Color ColorForNode(MissionNodeType type)
    {
        switch (type)
        {
            case MissionNodeType.ArmorGate:
            case MissionNodeType.IronRain:
                return new Color(0.18f, 0.19f, 0.23f, 0.96f);
            case MissionNodeType.RaiderTrack:
            case MissionNodeType.VelocityNet:
                return new Color(0.11f, 0.20f, 0.24f, 0.96f);
            case MissionNodeType.ShieldColumn:
            case MissionNodeType.EmpCorridor:
                return new Color(0.15f, 0.17f, 0.25f, 0.96f);
            case MissionNodeType.CorelineStand:
                return new Color(0.28f, 0.09f, 0.12f, 0.96f);
            default:
                return panelSoftColor;
        }
    }

    void SelectCampaignLevel(LevelDefinition level)
    {
        if (level == null) return;
        selectedCampaignLevel = level;
        AudioManager.PlaySfx(SfxType.UiClick);
        RefreshLevelButtons();
        FocusCampaignLevel(level);
    }

    void FocusCampaignLevel(LevelDefinition level)
    {
        if (level == null || campaignMapScroll == null || campaignMapPanel == null || campaignMapContent == null) return;

        int index = CampaignLevelIndex(level);
        if (index < 0) return;

        Canvas.ForceUpdateCanvases();
        float contentWidth = Mathf.Max(CampaignMapMinContentWidth, campaignMapContent.rect.width);
        float viewportWidth = Mathf.Max(1f, campaignMapPanel.rect.width);
        float scrollableWidth = Mathf.Max(1f, contentWidth - viewportWidth);
        float targetX = CampaignNodePosition(index).x - viewportWidth * 0.45f;
        campaignMapScroll.horizontalNormalizedPosition = Mathf.Clamp01(targetX / scrollableWidth);
    }

    int CampaignLevelIndex(LevelDefinition level)
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null || level == null) return -1;

        for (int i = 0; i < levelManager.LevelCount; i++)
            if (levelManager.GetLevelAt(i) == level)
                return i;

        return -1;
    }

    void RefreshCampaignDetail()
    {
        if (missionDetailPanel == null) return;

        LevelManager levelManager = LevelManager.Instance;
        LevelDefinition level = selectedCampaignLevel != null ? selectedCampaignLevel : (levelManager != null ? levelManager.currentLevel : null);
        if (level == null)
        {
            missionStatusText.text = "NO MISSION";
            missionTitleText.text = "";
            missionTypeText.text = "";
            missionBriefingText.text = "";
            missionEnemyMixText.text = "";
            missionToolsText.text = "";
            missionPressureText.text = "";
            missionRewardText.text = "";
            if (missionDevModeText != null) missionDevModeText.text = "";
            missionDeployButton.interactable = false;
            return;
        }

        bool current = levelManager != null && level == levelManager.currentLevel;
        bool unlocked = levelManager != null && levelManager.IsLevelUnlocked(level);
        bool completed = PlayerProgress.IsLevelCompleted(level);
        EnemyMix mix = CampaignIntel.BuildLevelMix(level);
        MissionNodeType nodeType = CampaignIntel.NodeTypeFor(level);

        missionStatusText.text = BuildLevelStatus(level, current, completed, unlocked);
        missionStatusText.color = unlocked ? accentColor : disabledColor;
        if (missionDevModeText != null)
            missionDevModeText.text = levelManager != null && levelManager.unlockAllLevelsForTesting ? "TEST MODE: ALL MISSIONS UNLOCKED" : "";
        missionTitleText.text = $"{level.levelNumber:00}  {level.displayName}";
        missionTypeText.text = CampaignIntel.NodeTypeLabel(nodeType);
        missionBriefingText.text = BuildLevelDetail(level, unlocked, completed);
        missionEnemyMixText.text = $"Enemy mix\n{CampaignIntel.BuildMixLabel(mix)}";
        missionToolsText.text = $"Recommended tools\n{CampaignIntel.BuildRecommendedTools(mix)}";
        missionPressureText.text = $"Pressure score: {CampaignIntel.PressureScore(level)}";
        missionRewardText.text = completed && !string.IsNullOrWhiteSpace(level.completionReward)
            ? level.completionReward
            : "Clear the sector to advance the campaign route.";

        missionDeployButton.interactable = unlocked;
        if (missionDeployButtonText != null)
            missionDeployButtonText.text = current ? "RESTART SECTOR" : "DEPLOY";
    }

    void OpenLevelSelect()
    {
        AudioManager.PlaySfx(SfxType.UiClick);
        RebuildDynamicUiIfNeeded();
        if (selectedCampaignLevel == null && LevelManager.Instance != null)
            selectedCampaignLevel = LevelManager.Instance.currentLevel;
        RefreshLevelButtons();

        wasPausedBeforeLevelSelect = isPaused;
        bool terminal = GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon);
        if (!terminal)
            SetPaused(true);

        if (levelSelectOverlay != null)
            levelSelectOverlay.SetActive(true);

        FocusCampaignLevel(selectedCampaignLevel);
    }

    void CloseLevelSelect()
    {
        AudioManager.PlaySfx(SfxType.UiClick);

        if (levelSelectOverlay != null)
            levelSelectOverlay.SetActive(false);

        bool terminal = GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsWon);
        if (!terminal)
            SetPaused(wasPausedBeforeLevelSelect);
    }

    void SelectLevel(LevelDefinition level)
    {
        if (LevelManager.Instance == null) return;

        AudioManager.PlaySfx(SfxType.UiClick);
        LevelManager.Instance.SelectLevelAndReload(level);
    }

    void DeploySelectedMission()
    {
        if (LevelManager.Instance == null || selectedCampaignLevel == null) return;
        SelectLevel(selectedCampaignLevel);
    }
}
