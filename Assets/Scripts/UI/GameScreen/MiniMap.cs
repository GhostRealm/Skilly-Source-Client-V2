using System;
using System.Collections.Generic;
using Game;
using Game.Entities;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.GameScreen
{
    public class MiniMap : MonoBehaviour
    {
        private const int _MAP_CHUNK_SIZE = 256;

        private Map _map;
        private MiniMapChunk[,] _chunks;

        private HashSet<MiniMapChunk> _needUpdateChunks;

        [SerializeField]
        private MiniMapChunk _miniMapChunk;

        [SerializeField]
        private RectTransform _chunkLayer;

        private float _width;
        private float _height;

        private List<float> _zoomLevels;
        private int _zoomIndex;

        private void Awake()
        {
            _needUpdateChunks = new HashSet<MiniMapChunk>();
        }

        public void Init(Map map)
        {
            foreach (Transform child in _chunkLayer)
            {
                Destroy(child.gameObject);
            }
            
            _map = map;
            _width = ((RectTransform) transform).sizeDelta.x;
            _height = ((RectTransform) transform).sizeDelta.y;

            _zoomIndex = 0;
            _zoomLevels = new List<float>();
            var maxZoom = Mathf.Max(_width / map.Width, _height / map.Height);
            _zoomLevels.Add(maxZoom);
            for (var zoom = 1f; zoom < 1 / maxZoom; zoom *= 2)
            {
                _zoomLevels.Add(zoom);
            }
            OnZoomChange();

            var xChunks = Convert(map.Width - 1) + 1;
            var yChunks = Convert(map.Height - 1) + 1;
            _chunks = new MiniMapChunk[xChunks, yChunks];

            var width = map.Width;
            var height = map.Height;
            for (var y = 0; y < xChunks; y++)
            {
                for (var x = 0; x < yChunks; x++)
                {
                    var chunkWidth = Mathf.Min(_MAP_CHUNK_SIZE, width);
                    var chunkHeight = Mathf.Min(_MAP_CHUNK_SIZE, height);
                    var texture = new Texture2D(chunkWidth, chunkHeight);
                    texture.filterMode = FilterMode.Point;
                    var pixelArray = new Color[chunkWidth * chunkHeight];
                    for (var i = 0; i < pixelArray.Length; i++)
                    {
                        pixelArray[i] = Color.black;
                    }
                    texture.SetPixels(pixelArray);
                    texture.Apply();

                    var chunk = Instantiate(_miniMapChunk, _chunkLayer);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, chunkWidth, chunkHeight), Vector2.zero, 1);
                    chunk.Sprite = sprite;
                    chunk.RectTransform.localScale = new Vector3(1, 1, 1);// * (1 / maxZoom);
                    chunk.RectTransform.anchoredPosition = new Vector2(x * _MAP_CHUNK_SIZE, y * _MAP_CHUNK_SIZE);// * (1 / maxZoom);
                    chunk.RectTransform.sizeDelta = new Vector2(chunkWidth, chunkHeight);

                    _chunks[x, y] = chunk;
                    width -= _MAP_CHUNK_SIZE;
                }

                width = map.Width;
                height -= _MAP_CHUNK_SIZE;
            }
        }

        public void SetGroundTile(int x, int y, ushort tileType)
        {
            var color = AssetLibrary.GetTileColor(tileType);
            SetPixel(x, y, color);
        }

        public void SetEntity(int x, int y, ushort entityType)
        {
            var color = AssetLibrary.GetObjectColor(entityType);
            SetPixel(x, y, color);
        }

        private void SetPixel(int x, int y, Color color)
        {
            var chunkX = Convert(x);
            var chunkY = Convert(y);
            var chunk = _chunks[chunkX, chunkY];
            var textureX = x % _MAP_CHUNK_SIZE;
            var textureY = y % _MAP_CHUNK_SIZE;
            chunk.Sprite.texture.SetPixel(textureX, textureY, color);
            _needUpdateChunks.Add(chunk);
        }

        private static int Convert(float value) => (int)Math.Floor(value / _MAP_CHUNK_SIZE);

        public void Apply()
        {
            foreach (var chunk in _needUpdateChunks)
            {
                chunk.Sprite.texture.Apply();
            }
            
            _needUpdateChunks.Clear();
        }

        private void OnZoomChange()
        {
            _chunkLayer.localScale = new Vector3(1, 1, 1) * _zoomLevels[_zoomIndex];
        }

        private void Update()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                if (_zoomIndex < _zoomLevels.Count - 1)
                {
                    _zoomIndex++;
                    OnZoomChange();
                }
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                if (_zoomIndex > 0)
                {
                    _zoomIndex--;
                    OnZoomChange();
                }
            }

            if (_map?.MyPlayer != null)
            {
                // if (Input.GetKeyDown(KeyCode.LeftArrow))
                // {
                //     var pos = _chunkLayer.localPosition;
                //     pos.x -= _zoomLevels[_zoomIndex];
                //     _chunkLayer.localPosition = pos;
                // }
                // if (Input.GetKeyDown(KeyCode.RightArrow))
                // {
                //     var pos = _chunkLayer.localPosition;
                //     pos.x += _zoomLevels[_zoomIndex];
                //     _chunkLayer.localPosition = pos;
                // }
                var pos = -_map.MyPlayer.Position;
                pos *= _zoomLevels[_zoomIndex];
                pos += new Vector2(96, -96);
                _chunkLayer.localPosition = pos;
                
                // var rect = ((RectTransform) transform).rect;
                // var chunkRect = new Rect(_chunkLayer.anchoredPosition, _chunkLayer.sizeDelta);
                // var tx = 0f;
                // if (chunkRect.xMin > rect.xMin)
                // {
                //     tx = rect.xMin - chunkRect.xMin;
                // }
                // else if (chunkRect.xMax < rect.xMax)
                // {
                //     tx = rect.xMax - chunkRect.xMax;
                // }
                //
                // var ty = 0f;
                // if (chunkRect.yMin > rect.yMin)
                // {
                //     ty = chunkRect.yMin - rect.yMin;
                // }
                // else if (chunkRect.yMax < rect.yMax)
                // {
                //     ty = chunkRect.yMax - rect.yMax;
                // }
                //
                // _chunkLayer.anchoredPosition += new Vector2(tx, ty);
            }
                
        }
    }
}