using Game.Player;
using UnityEngine;

namespace Game
{
    public class PlayerAudioPlayer : MonoBehaviour
    {
        public enum PlayerAudioType
        {
            Footstep,
            Jump,
            Landing,
            HookShotAttach,
            HookShotDetach
        }

        [SerializeField] private Player.Action.HookShotAction hookShotAction;
        [SerializeField] private PlayerState playerState;

        [SerializeField] private AudioSource footstepAudio;
        [SerializeField] private AudioSource jumpAudio;
        [SerializeField] private AudioSource landingAudio;
        [SerializeField] private AudioSource hookShotAttachAudio;
        [SerializeField] private AudioSource hookShotDetachAudio;

        private void Start()
        {
            hookShotAction.OnHookShotActivated += () => { Play(PlayerAudioType.HookShotAttach); };
            hookShotAction.OnHookShotDeactivated += () => { Play(PlayerAudioType.HookShotDetach); };

            playerState.OnJump += () => { Play(PlayerAudioType.Jump); };
            playerState.OnLand += () => { Play(PlayerAudioType.Landing); };
        }

        public void Play(PlayerAudioType type)
        {
            GetAudioSource(type).Play();
        }

        public AudioSource GetAudioSource(PlayerAudioType type)
        {
            return type switch
            {
                PlayerAudioType.Footstep => footstepAudio,
                PlayerAudioType.Jump => jumpAudio,
                PlayerAudioType.Landing => landingAudio,
                PlayerAudioType.HookShotAttach => hookShotAttachAudio,
                PlayerAudioType.HookShotDetach => hookShotDetachAudio,
                _ => null
            };
        }
    }
}