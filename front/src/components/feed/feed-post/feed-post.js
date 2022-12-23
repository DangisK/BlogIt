import { MoreVert } from "@mui/icons-material";
import { Avatar, Card, CardContent, CardHeader, Divider, Grid, Typography } from "@mui/material";
import React, { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppContext } from "../../../context";
import { stringToColor } from "../../../utils/utils";
import { EditPostModal } from "../edit-post-modal";

export const FeedPost = ({ id, username, name, body, creationDate, comments }) => {
  const { user } = useContext(AppContext);
  const navigateTo = useNavigate();

  return (
    <Card sx={{ margin: 3 }} onClick={() => navigateTo(`/posts/${id}`)}>
      <CardHeader
        avatar={
          <Avatar sx={{ bgcolor: `${stringToColor(username)}` }}>{username.charAt(0)}</Avatar>
        }
        title={
          <Typography variant="h7" sx={{ wordWrap: "break-word" }}>
            {username}
          </Typography>
        }
        subheader={new Date(creationDate).toLocaleDateString("zh-Hans-CN", {
          hour: "2-digit",
          minute: "2-digit",
        })}
      />
      <CardContent sx={{ paddingTop: 0 }}>
        <Divider />
        <Typography
          variant="h5"
          color="text.primary"
          sx={{ marginTop: "10px", wordWrap: "break-word" }}
        >
          {name}
        </Typography>
        <Typography
          variant="body1"
          color="text.primary"
          sx={{ marginTop: "20px", wordWrap: "break-word" }}
        >
          {body}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ marginTop: "25px" }}>
          {!!comments ? comments.length : 0} comments
        </Typography>
      </CardContent>
    </Card>
  );
};
