using UnityEngine;
using UnityEngine.UI;
using System.Collections;

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class AutoScrollRect : MonoBehaviour
{
		/// <summary>
		/// The scroll rect reference.
		/// </summary>
		public ScrollRect scrollRect;

		/// <summary>
		/// The auto scroll speed.
		/// </summary>
		[Range (0, 500)]
		public float autoScrollSpeed = 50;

		/// <summary>
		/// Whether the script is running.
		/// </summary>
		public bool isRunning = true;

		/// <summary>
		/// Whether the pointer is down.
		/// </summary>
		private bool isPointerDown;

		/// <summary>
		/// The scroll range.
		/// </summary>
		public Vector2 scrollRange;

		/// <summary>
		/// The scroll rect content Anchored postion.
		/// </summary>
		private Vector2 scrollRectContentAPostion;

		void Start ()
		{
				if (scrollRect != null) {
						///Set the inital postion of the content
						Vector2 anchoredPostion = scrollRect.content.anchoredPosition;
						anchoredPostion.y = scrollRange.x;
						scrollRect.content.anchoredPosition = anchoredPostion;
				}
		}

		void Update ()
		{
				if (!isRunning || scrollRect == null) {
						return;
				}

				if (!isPointerDown && scrollRect.content != null) {
						AutoScroll ();
				}
		}

		/// <summary>
		/// Automatically scroll the content.
		/// </summary>
		private void AutoScroll ()
		{
				scrollRectContentAPostion = scrollRect.content.anchoredPosition;
				scrollRectContentAPostion.y += autoScrollSpeed * Time.smoothDeltaTime;

				if (scrollRectContentAPostion.y >= scrollRange.y) {
						scrollRectContentAPostion.y = scrollRange.x;
				} else if (scrollRectContentAPostion.y <=  scrollRange.x) {
						scrollRectContentAPostion.y =  scrollRange.y;
				}
				scrollRect.content.anchoredPosition = scrollRectContentAPostion;
		}

		/// <summary>
		/// On pointer down event.
		/// </summary>
		public void OnPointerDown ()
		{
				isPointerDown = true;
		}

		/// <summary>
		/// On pointer up event.
		/// </summary>
		public void OnPointerUp ()
		{
				isPointerDown = false;
		}
}
