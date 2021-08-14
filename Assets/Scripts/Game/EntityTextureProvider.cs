using Game.Entities;
using Models.Static;
using UnityEngine;
using Utils;

namespace Game
{
    public class EntityTextureProvider : ITextureProvider
    {
        private const int _ATTACK_PERIOD = 500;
        private readonly Entity _entity;

        private float _facing;

        private Sprite _portrait;

        public EntityTextureProvider(Entity entity)
        {
            _entity = entity;
        }

        public Sprite GetTexture(int time)
        {
            Sprite image;
            if (_entity.Desc.TextureData.Animation != null)
            {
                var p = 0f;
                var action = Action.Stand;
                if (time < _entity.AttackStart + _ATTACK_PERIOD)
                {
                    if (!_entity.Desc.DontFaceAttacks)
                    {
                        _facing = _entity.AttackAngle;
                    }

                    p = (time - _entity.AttackStart) % (float)_ATTACK_PERIOD / _ATTACK_PERIOD;
                    action = Action.Attack;
                }
                else if (_entity.Direction != Vector2.zero)
                {
                    var walkPer = (int)(0.5f / (_entity.Direction.magnitude * 4));
                    walkPer = 400 - walkPer % 400;
                    _facing = Mathf.Atan2(_entity.Direction.y, _entity.Direction.x);
                    action = Action.Walk;
                    p = time % walkPer / walkPer;
                }

                image = _entity.Desc.TextureData.Animation.ImageFromFacing(_facing, action, p);
            }
            else
            {
                image = _entity.Desc.TextureData.Texture;
            }

            return SpriteUtils.Redraw(image, _entity.Size);
        }

        public Sprite GetPortrait()
        {
            if (_portrait == null)
            {
                var portraitTexture = _entity.Desc.Portrait != null
                    ? _entity.Desc.Portrait.Texture
                    : _entity.Desc.TextureData.Texture;
                var size = 4 / (int)portraitTexture.rect.width * 100;
                _portrait = SpriteUtils.Redraw(portraitTexture, size);
            }

            return _portrait;
        }
    }
}