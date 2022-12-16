#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweetDisplay : MonoBehaviour
{
    public Image authorImg;
    public Text authorName;
    public Text postTime;
    public Text messageContent;
    public Text likesText;
	private TweetDataInstance data;

    public void Setup(TweetDataInstance newData)
    {
    	data = newData;
        TweetData content = GameController.GetInstance().gameAssets.FindTweet(data.contentID);
    	authorImg.sprite = content.author.thumbnail;
    	authorName.text = content.author.characterName;
    	postTime.text = data.publishingDate.Day.ToString() + '.' + data.publishingDate.Month.ToString() + '.' + data.publishingDate.Year.ToString() + ' ' + data.publishingDate.Hour.ToString() + ':' + data.publishingDate.Minute.ToString();
    	messageContent.text = content.messageText;
    	likesText.text = data.likeAmount.ToString() + " likes";
    }

}
