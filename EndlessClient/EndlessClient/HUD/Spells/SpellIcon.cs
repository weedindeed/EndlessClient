﻿// Original Work Copyright (c) Ethan Moffat 2014-2016
// This file is subject to the GPL v2 License
// For additional details, see the LICENSE file

using System;
using EOLib;
using EOLib.Graphics;
using EOLib.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EndlessClient.HUD.Spells
{
	public class SpellIcon : EmptySpellIcon
	{
		private bool _selected;
		public override bool Selected
		{
			get { return _selected; }
			set
			{
				_selected = value;
				if (_selected)
					OnSelected();
			}
		}

		private short _level;
		public override short Level
		{
			get { return _level; }
			set
			{
				if (_level != value)
				{
					_level = value;
					OnLevelChanged();
				}
			}
		}

		public override bool IsDragging { get { return _dragging; } }

		public override SpellRecord SpellData { get { return _spellData; } }

		//stops the base class update logic from being called
		protected override bool DoEmptySpellIconUpdateLogic { get { return false; } }
		private readonly Texture2D _spellGraphic, _spellLevelColor;
		private readonly SpellRecord _spellData;

		private Rectangle _spellGraphicSourceRect;
		private DateTime _clickTime;
		private bool _dragging, _followMouse;
		private Rectangle _levelDestinationRectangle;

		public SpellIcon(ActiveSpells parent, SpellRecord data, int slot)
			: base(parent, slot)
		{
			_spellData = data;
			_spellGraphic = ((EOGame)Game).GFXManager.TextureFromResource(GFXTypes.SpellIcons, _spellData.Icon);
			_spellGraphicSourceRect = new Rectangle(0, 0, _spellGraphic.Width / 2, _spellGraphic.Height);

			_spellLevelColor = new Texture2D(Game.GraphicsDevice, 1, 1);
			_spellLevelColor.SetData(new[] {Color.FromNonPremultiplied(0xc9, 0xb8, 0x9b, 0xff)});
			OnLevelChanged();

			_clickTime = DateTime.Now;
		}

		public override void Update(GameTime gameTime)
		{
			if (!ShouldUpdate()) return;

			UpdateIconSourceRect();
			DoClickAndDragLogic();

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible) return;

			SpriteBatch.Begin();
			DrawSpellIcon();
			DrawSpellLevel();
			SpriteBatch.End();

			base.Draw(gameTime);
		}

		protected override void OnSlotChanged()
		{
			base.OnSlotChanged();
			if (_spellGraphic != null)
				SetIconHover(MouseOver);
			OnLevelChanged();
		}

		private void OnSelected()
		{
			var hud = ((EOGame) Game).Hud;
			switch (SpellData.Target)
			{
				case SpellTarget.Normal:
					hud.SetStatusLabel(DATCONST2.SKILLMASTER_WORD_SPELL, SpellData.Name, DATCONST2.SPELL_WAS_SELECTED);
					break;
				case SpellTarget.Group:
					if(!hud.MainPlayerIsInParty())
						hud.SetStatusLabel(DATCONST2.STATUS_LABEL_TYPE_WARNING, DATCONST2.SPELL_ONLY_WORKS_ON_GROUP);
					break;
			}
		}

		private void OnLevelChanged()
		{
			//36 is full width of level bar
			var width = (int)(Level / 100.0 * 36);
			_levelDestinationRectangle = new Rectangle(DrawAreaWithOffset.X + 3, DrawAreaWithOffset.Y + 40, width, 6);
		}

		private void UpdateIconSourceRect()
		{
			if (MouseOver && !MouseOverPreviously ||
				MouseOverPreviously && !MouseOver)
			{
				SetIconHover(MouseOver);
				if (MouseOver && !_parentSpellContainer.AnySpellsDragging())
					((EOGame) Game).Hud.SetStatusLabel(DATCONST2.SKILLMASTER_WORD_SPELL, SpellData.Name);
			}
		}

		private void SetIconHover(bool hover)
		{
			var halfWidth = _spellGraphic.Width/2;
			_spellGraphicSourceRect = new Rectangle(hover ? halfWidth : 0, 0, halfWidth, _spellGraphic.Height);
		}

		private void DoClickAndDragLogic()
		{
			if (!_dragging && _parentSpellContainer.AnySpellsDragging())
				return;

			var currentState = Mouse.GetState();
			if (LeftButtonDown(currentState))
			{
				if (!_dragging)
				{
					_followMouse = true;
					_clickTime = DateTime.Now;
					_parentSpellContainer.SetSelectedSpellBySlot(Slot);
				}
				else
				{
					EndDragging();
				}
			}
			else if (LeftButtonUp(currentState))
			{
				if (!_dragging)
				{
					var clickDelta = (DateTime.Now - _clickTime).TotalMilliseconds;
					if (clickDelta < 75)
					{
						_dragging = true;
					}
				}
				else
				{
					EndDragging();
				}
			}

			if (!_dragging && _followMouse && (DateTime.Now - _clickTime).TotalMilliseconds >= 75)
				_dragging = true;
		}

		private bool LeftButtonDown(MouseState currentState)
		{
			return MouseOver && MouseOverPreviously &&
				   currentState.LeftButton == ButtonState.Pressed &&
				   PreviousMouseState.LeftButton == ButtonState.Released;
		}

		private bool LeftButtonUp(MouseState currentState)
		{
			return currentState.LeftButton == ButtonState.Released &&
				   PreviousMouseState.LeftButton == ButtonState.Pressed;
		}

		private void EndDragging()
		{
			_dragging = false;
			_followMouse = false;

			var newSlot = GetCurrentHoverSlot();
			_parentSpellContainer.MoveItem(this, newSlot);
		}

		private int GetCurrentHoverSlot()
		{
			return _parentSpellContainer.GetCurrentHoverSlot();
		}

		private void DrawSpellIcon()
		{
			Rectangle targetDrawArea;
			Color alphaColor;
			if (!_followMouse)
			{
				targetDrawArea = new Rectangle(
					DrawAreaWithOffset.X + (DrawAreaWithOffset.Width - _spellGraphicSourceRect.Width) / 2,
					DrawAreaWithOffset.Y + (DrawAreaWithOffset.Height - _spellGraphicSourceRect.Height) / 2,
					_spellGraphicSourceRect.Width,
					_spellGraphicSourceRect.Height);
				alphaColor = Color.White;
			}
			else
			{
				targetDrawArea = new Rectangle(
					Mouse.GetState().X - _spellGraphicSourceRect.Width / 2,
					Mouse.GetState().Y - _spellGraphicSourceRect.Height / 2,
					_spellGraphicSourceRect.Width,
					_spellGraphicSourceRect.Height
					);
				alphaColor = Color.FromNonPremultiplied(255, 255, 255, 128);
			}

			if (targetDrawArea.Width*targetDrawArea.Height == 0)
				return;

			SpriteBatch.Draw(_spellGraphic, targetDrawArea, _spellGraphicSourceRect, alphaColor);
		}

		private void DrawSpellLevel()
		{
			if (_followMouse || _dragging || _spellLevelColor == null)
				return;

			SpriteBatch.Draw(_spellLevelColor, _levelDestinationRectangle, Color.White);
		}
	}
}
