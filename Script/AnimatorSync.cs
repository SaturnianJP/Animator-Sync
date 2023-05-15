
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class AnimatorSync: UdonSharpBehaviour
{
    public Animator _animator;

    [SerializeField]
    [UdonSynced]
    public string[] parameterNames;

    [SerializeField]
    [UdonSynced]
    public string[] values;

    [SerializeField]
    [UdonSynced]
    public int[] playingStates;

    private void Start()
    {
        Debug.Log($"[<color=green>AnimatorReload</color>] {this.name} Animator Sync Started.");
    }

    private void OnValidate()
    {
        _animator = this.GetComponent<Animator>();
    }

    private void ReloadParameters()
    {
        if (_animator == null)
            return;

        parameterNames = new string[_animator.parameters.Length];
        values = new string[_animator.parameters.Length];
        playingStates = new int[_animator.parameters.Length];

        //パラメータリストを取得
        for (int i = 0; i < _animator.parameters.Length; i++)
        {
            var param = _animator.parameters[i];

            parameterNames[i] = param.name;

            if (param.type == AnimatorControllerParameterType.Float)
            {
                values[i] = $"Float, {_animator.GetFloat(param.name)}";
            }
            else if (param.type == AnimatorControllerParameterType.Int)
            {
                values[i] = $"Int, {_animator.GetInteger(param.name)}";
            }
            else if (param.type == AnimatorControllerParameterType.Bool)
            {
                values[i] = $"Bool, {_animator.GetBool(param.name)}";
            }
        }

        //プレイ中のステートを取得
        for (int i = 0; i < _animator.layerCount; i++)
        {
            var stateHash = _animator.GetCurrentAnimatorStateInfo(i).shortNameHash;
            playingStates[i] = stateHash;
        }

        Debug.Log($"[<color=green>AnimatorReload</color>Send : Sync Parameters]");
    }

    private void SyncParameters()
    {
        if (_animator == null)
        {
            Debug.Log($"[<color=red>AnimatorReload</color>]Error : Animator is null");
            return;
        }

        //パラメータの同期
        for (int i = 0; i < parameterNames.Length; i++)
        {
            var name = parameterNames[i];
            var vl = values[i].Split(',');
            if (vl.Length < 2)
                continue;

            string type = vl[0];
            string value = vl[1];

            if (type == "Float")
            {
                float _float;
                if (float.TryParse(value, out _float))
                    _animator.SetFloat(name, _float);
            }
            else if (type == "Int")
            {
                int _int;
                if (int.TryParse(value, out _int))
                    _animator.SetInteger(name, _int);
            }
            else if (type == "Bool")
            {
                bool _bool;
                if (bool.TryParse(value, out _bool))
                    _animator.SetBool(name, _bool);
            }
        }

        for (int i = 0; i < playingStates.Length; i++)
        {
            int hash = playingStates[i];

            if (_animator.GetCurrentAnimatorStateInfo(i).shortNameHash != hash)
                _animator.Play(hash, i);    
        }

        Debug.Log($"[<color=green>AnimatorReload</color>]{_animator.name} is Synced.");
    }

    //マスターから参加者へ同期時
    public override void OnDeserialization()
    {
        //パラメータとステートを同期
        SyncParameters();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        Debug.Log($"[<color=green>AnimatorReload</color>]{player.displayName} Joined.");

        //マスター以外は弾く
        if (player == null || player == Networking.LocalPlayer)
            return;

        if (Networking.LocalPlayer.isMaster)
        {
            //パラメータ情報を同期用に格納
            ReloadParameters();

            //同期
            RequestSerialization();
        }
    }
}
