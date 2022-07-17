using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ToonBoom.Harmony;

public class CharacterControl : MonoBehaviour
{
    private HarmonyRenderer _harmonyRenderer;
    private Animator _animator;

    public HarmonyProject defaultChracterProject;
    public RuntimeAnimatorController defaultCharacterAnimatorController;

    public HarmonyProject newCharacterProject;
    public RuntimeAnimatorController newCharacterAnimatorController;

    // Start is called before the first frame update
    void Start()
    {
        _harmonyRenderer = GetComponent<HarmonyRenderer>();
        _animator = GetComponent<Animator>();

        // 하모니 캐릭터에 대한 프로젝트 애셋을 확인
        var harmonyProject = _harmonyRenderer.Project;

        // 애니메이션 클립 목록 출력
        foreach(var clip in harmonyProject.Clips)
        {
            Debug.Log($"애니메이션 클립 : {clip.DisplayName}");
        }

        // 현재 활성화된 애니메이션
        Debug.Log($"현재 애니메이션 클립 번호 : {_harmonyRenderer.CurrentClipIndex}");
        Debug.Log($"현재 애니메이션 클립 이름 : {_harmonyRenderer.CurrentClip.DisplayName}");

        // 스킨을 할당할수 있는 그룹 목록을 출력
        for(var i = 0; i < harmonyProject.Groups.Count; i++)
        {
            var group = harmonyProject.Groups[i];
            Debug.Log($"그룹 아이디 : {i}, 그룹 이름 : {group}");
        }

        // 스킨 목록을 출력
        for(var i = 0; i < harmonyProject.Skins.Count; i++)
        {
            var skin = harmonyProject.Skins[i];
            Debug.Log($"스킨 아이디 : {i}, 스킨 이름 : {skin}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        _animator.SetFloat("Speed", horizontalInput);

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetupCharacter(defaultChracterProject, defaultCharacterAnimatorController);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetupCharacter(newCharacterProject, newCharacterAnimatorController);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(IsSkinActivated("Glasses", "Sunglasses"))
            {
                SetSkin("Glasses", "Glasses");    
            }
            else
            {
                SetSkin("Glasses", "Sunglasses");
            }
            
        }
    }

    public int GetGroupSkinIndex(string groupName)
    {
        var groupId = _harmonyRenderer.Project.Groups.IndexOf(groupName);

        for(var i = 0; i < _harmonyRenderer.GroupSkins.Count; i++)
        {
            var groupSkin = _harmonyRenderer.GroupSkins[i];
            if(groupSkin.GroupId == groupId)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetSkin(string groupName, string skinName) {
        var skinId = _harmonyRenderer.Project.Skins.IndexOf(skinName); // 스킨 ID 가져오기
        var groupSkinIndex = GetGroupSkinIndex(groupName);

        _harmonyRenderer.GroupSkins.SetSkin(groupSkinIndex, skinId);

        _harmonyRenderer.UpdateRenderer();
    }

    public bool IsSkinActivated(string groupName, string skinName)
    {
        var skinId = _harmonyRenderer.Project.Skins.IndexOf(skinName);
        var groupId = _harmonyRenderer.Project.Groups.IndexOf(groupName);

        return _harmonyRenderer.GroupSkins.Any(groupSkin => groupSkin.GroupId == groupId && groupSkin.SkinId == skinId);
    }

    public void SetupCharacter(HarmonyProject characterProject, RuntimeAnimatorController animatorController)
    {
        _harmonyRenderer.Project = characterProject;
        _animator.runtimeAnimatorController = animatorController;

        _harmonyRenderer.bypassForSkins();
        _harmonyRenderer.UpdateRenderer();
    }
}
