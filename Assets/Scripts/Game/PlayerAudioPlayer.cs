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

        [SerializeField] private HookShotAction hookShotAction;
        [SerializeField] private PlayerController playerController;

        [SerializeField] private AudioSource footstepAudio;
        [SerializeField] private AudioSource jumpAudio;
        [SerializeField] private AudioSource landingAudio;
        [SerializeField] private AudioSource hookShotAttachAudio;
        [SerializeField] private AudioSource hookShotDetachAudio;

        private void Start()
        {
            hookShotAction.OnActivated += () => { Play(PlayerAudioType.HookShotAttach); };
            hookShotAction.OnDeactivated += () => { Play(PlayerAudioType.HookShotDetach); };

            playerController.OnJump += v =>
            {
                if (v == PlayerController.EventContext.Begin) Play(PlayerAudioType.Jump);
            };

            playerController.OnWallJumping += () => { Play(PlayerAudioType.Jump); };
            playerController.OnLanding += () => { Play(PlayerAudioType.Landing); };
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