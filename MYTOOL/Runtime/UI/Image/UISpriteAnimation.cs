using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MYTOOL.UI
{
    [RequireComponent(typeof(Image))]
    public class UISpriteAnimation : MonoBehaviour
    {
        [SerializeField]
        private byte FPS = 10;
        [SerializeField]
        private bool AutoPlay = true;
        [SerializeField]
        public bool Foward = true;
        [SerializeField]
        public bool Loop = true;

        [Space]
        [SerializeField]
        private List<Sprite> SpriteFrames;


        private Image ImageSource;
        private int mCurFrame;
        private float mDelta;

        public bool IsPlaying { get; private set; }

        public int FrameCount
        {
            get
            {
                return SpriteFrames.Count;
            }
        }

        void Awake()
        {
            ImageSource = GetComponent<Image>();
            IsPlaying = AutoPlay;
        }

        void Start()
        {
            mCurFrame = Foward ? 0 : FrameCount - 1;
            SetSprite(mCurFrame);
        }

        void Update()
        {
            if (!IsPlaying || 0 == FrameCount) return;

            mDelta += Time.deltaTime;
            if (mDelta > 1f / FPS)
            {
                mDelta = 0;

                mCurFrame += Foward ? 1 : -1;

                if (mCurFrame >= FrameCount || mCurFrame < 0)
                {
                    if (Loop)
                    {
                        mCurFrame = mCurFrame >= FrameCount ? 0 : FrameCount - 1;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }

                SetSprite(mCurFrame);
            }
        }

        public UISpriteAnimation Play()
        {
            IsPlaying = true;
            return this;
        }

        public UISpriteAnimation Pause()
        {
            IsPlaying = false;
            return this;
        }

        public UISpriteAnimation Resume()
        {
            IsPlaying = true;
            return this;
        }

        public UISpriteAnimation Stop()
        {
            mCurFrame = Foward ? 0 : FrameCount - 1;
            SetSprite(mCurFrame);
            IsPlaying = false;
            return this;
        }

        private void SetSprite(int idx)
        {
            ImageSource.sprite = SpriteFrames[idx];
            ImageSource.SetNativeSize();
        }
    }
}