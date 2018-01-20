#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/poll.h>
#include <sys/socket.h>
#include <sys/time.h>
#include <netinet/in.h>
#include <errno.h>
#include <unistd.h>
#include <string.h>
#include <algorithm>
#include <iostream>
#include <map>
#include <iterator>
#include <vector>

#define SERVER_PORT  12345

#define TRUE             1
#define FALSE            0

using namespace std;
map<int, string> red_sd;
map<int, string> blue_sd;

string table = "nnnnnnnnn";
string msg;

int red_voted = 0, blue_voted = 0;

//Remove disconected player from lists
void playerDisconnected(int &sd){
  close(sd);
  if(red_sd.count(sd) > 0){
    printf("Removing player from Red Team...\n");
    red_sd.erase(sd);
    printf("  Size of Red Team: %d\n", (int)red_sd.size());

  }
  if(blue_sd.count(sd) > 0){
    printf("Removing player from Blue Team...\n");
    blue_sd.erase(sd);
    printf("  Size of Blue Team: %d\n", (int)blue_sd.size());
  }
  sd = -1;
}

void sendVote(int team){
  char buff[12];
    // if(team){ //Send vote to Red Team
    //     for(map<int, string>::iterator it = red_sd.begin(); it != red_sd.end(); it++){
    //       sprintf(buff, "v%sO", table.c_str());
    //       send(it->first, table.c_str(), 11, 0);
    //     }
    // } else { //Send vote to Blue Team
    //     for(map<int, string>::iterator it = blue_sd.begin(); it != blue_sd.end(); it++){
    //       sprintf(buff, "v%sX", table.c_str());
    //       send(it->first, table.c_str(), 11, 0);
    //     }
    // }
    //Send vote to Red Team
        for(map<int, string>::iterator it = red_sd.begin(); it != red_sd.end(); it++){
          sprintf(buff, "v%sO", table.c_str());
          send(it->first, table.c_str(), 11, 0);
        }
    //Send vote to Blue Team
        for(map<int, string>::iterator it = blue_sd.begin(); it != blue_sd.end(); it++){
          sprintf(buff, "v%sX", table.c_str());
          send(it->first, table.c_str(), 11, 0);
        
    }
}

void decideVote(int team){//Choose most common vote (0 - Red, 1 - Blue)
  int max_cnt = 0, tmp_cnt = 0;
    if(team == 0){ 
      for(map<int, string>::iterator it = red_sd.begin(); it != red_sd.end(); it++){
        for(map<int, string>::iterator it2 = red_sd.begin(); it2 != red_sd.end(); it2++){
          if(it->second.compare(it2->second) == 0) 
            tmp_cnt++;
          if(tmp_cnt > max_cnt){
            max_cnt = tmp_cnt;
            table = it->second;
          }
        }
      }
     // printf("Most common red vote (Player %d Cnt:%d): %s\n", max_cnt_id, max_cnt, it->second.c_str());
      sendVote(0); 
  } else { //Choose most common vote in Blue Team
      for(map<int, string>::iterator it = blue_sd.begin(); it != blue_sd.end(); it++){
        for(map<int, string>::iterator it2 = blue_sd.begin(); it2 != blue_sd.end(); it2++){
          if(it->second.compare(it2->second) == 0) 
            tmp_cnt++;
          if(tmp_cnt > max_cnt){
            max_cnt = tmp_cnt;
            table = it->second;
          }
        }
      }
      //printf("Most common blue vote (Player %d Cnt:%d): %s\n", max_cnt_id, max_cnt, it->second.c_str());
      sendVote(1);
    }
}

void setVote(int sd, char buf[11]){
  if(red_sd.count(sd) > 0){
    red_sd.at(sd) = string(buf, 11);
    //printf("\033[1;31m[VOTE]\033[0;31mRed player %d voted for: %s\n", sd, red_sd.at(sd).c_str() );
    table = red_sd.at(sd);
    red_voted++;
    if(red_voted >= (int)red_sd.size()){
      decideVote(0);
      red_voted = 0;
    }        
  }
  if(blue_sd.count(sd) > 0){
    blue_sd.at(sd) = string(buf, 11);
    //printf("\033[1;31m[VOTE]\033[0;31Blue player %d voted for: %s\n", sd, blue_sd.at(sd).c_str() );
    table = blue_sd.at(sd);
    blue_voted++;
    if(blue_voted >= (int)blue_sd.size()){
      decideVote(1);
      blue_voted = 0;
    }
  }
}

void restartGame(){
  table = "nnnnnnnnn";
  char buff[12];
  for(map<int, string>::iterator it = red_sd.begin(); it != red_sd.end(); it++){
    sprintf(buff, "r%sX", table.c_str());
    send(it->first, buff, 12, 0);
  }
  for(map<int, string>::iterator it = blue_sd.begin(); it != blue_sd.end(); it++){
    sprintf(buff, "r%sO", table.c_str());
    send(it->first, buff, 12, 0);
  }
}

void sendMessage(string msg){
  for(map<int, string>::iterator it = red_sd.begin(); it != red_sd.end(); it++){
    send(it->first, msg.c_str(), sizeof(msg.c_str()), 0);
  }
  for(map<int, string>::iterator it = blue_sd.begin(); it != blue_sd.end(); it++){
    send(it->first, msg.c_str(), sizeof(msg.c_str()), 0);
  }
}

int main (int argc, char *argv[])
{
  int    len, rc, on = 1;
  int    listen_sd = -1, new_sd = -1;
  int    end_server = FALSE, compress_array = FALSE;
  char   buffer[11];
  struct sockaddr_in   addr;
  int    timeout;
  struct pollfd fds[200];
  int    nfds = 1, current_size = 0, i, j;

  listen_sd = socket(AF_INET, SOCK_STREAM, 0);
  if (listen_sd < 0)
  {
    perror("socket() failed");
    exit(-1);
  }

  rc = setsockopt(listen_sd, SOL_SOCKET,  SO_REUSEADDR, (char *)&on, sizeof(on));
  if (rc < 0)
  {
    perror("setsockopt() failed");
    close(listen_sd);
    exit(-1);
  }

  rc = ioctl(listen_sd, FIONBIO, (char *)&on);
  if (rc < 0)
  {
    perror("ioctl() failed");
    close(listen_sd);
    exit(-1);
  }

  memset(&addr, 0, sizeof(addr));
  addr.sin_family      = AF_INET;
  addr.sin_addr.s_addr = htonl(INADDR_ANY);
  addr.sin_port        = htons(SERVER_PORT);
  rc = bind(listen_sd, (struct sockaddr *)&addr, sizeof(addr));
  if (rc < 0)
  {
    perror("bind() failed");
    close(listen_sd);
    exit(-1);
  }

  rc = listen(listen_sd, 32);
  if (rc < 0)
  {
    perror("listen() failed");
    close(listen_sd);
    exit(-1);
  }

  memset(fds, 0 , sizeof(fds));

  fds[0].fd = listen_sd;
  fds[0].events = POLLIN;

  timeout = (3 * 60 * 1000);

  do
  {
    printf("Waiting on poll()...\n");
    rc = poll(fds, nfds, timeout);

    if (rc < 0)
    {
      perror("  poll() failed");
      break;
    }

    if (rc == 0)
    {
      printf("  poll() timed out.  End program.\n");
      break;
    }

    current_size = nfds;
    for (i = 0; i < current_size; i++)
    {
      if(fds[i].revents == 0)
        continue;

      if(fds[i].revents != POLLIN)
      {
        printf("  Error! revents = %d\n", fds[i].revents);
        end_server = TRUE;
        break;

      }
      //New incoming connection
      if (fds[i].fd == listen_sd)
      {
        printf("  Listening socket is readable\n");
        do
        {
          new_sd = accept(listen_sd, NULL, NULL);
          if (new_sd < 0)
          {
            if (errno != EWOULDBLOCK)
            {
              perror("  accept() failed");
              end_server = TRUE;
            }
            break;
          }

          printf("  New incoming connection - %d\n", new_sd);
          fds[nfds].fd = new_sd;
          fds[nfds].events = POLLIN;

          //Add new player to team and send him his team (not sure if working)
          if(red_sd.size() <= blue_sd.size()){
            red_sd.insert(pair<int, string>(new_sd, "nnnnnnnnn"));
            send(new_sd, "tX", 3, 0);
            printf("New red player! \n");
          } else {
            blue_sd.insert(pair<int, string>(new_sd, "nnnnnnnnn"));
            send(new_sd, "tO", 3, 0);
            printf("New Blue player! \n");
          }

          nfds++;

        } while (new_sd != -1);
      }
      
      //Incoming message
      else
      {
        printf("  Descriptor %d is readable\n", fds[i].fd);
        // close_conn = FALSE;
          strcpy(buffer, "           ");
          rc = recv(fds[i].fd, buffer, sizeof(buffer), 0);
          if (rc < 0)
          {
            if (errno != EWOULDBLOCK)
            {
              perror("  recv() failed");
              // close_conn = TRUE;
            }
            break;
          }
          //Check if client closed connection
          if (rc == 0)
          {
            printf("  Connection closed\n");
            playerDisconnected(fds[i].fd);
            compress_array = TRUE;
            continue;
          }

          len = rc;
          printf("  %d bytes received: %s\n", len, buffer);
          msg.clear();
          //Consume message
          msg = buffer;
          //Check if msg is a vote
          if(msg.find("v") != string::npos){
              setVote(fds[i].fd, buffer);
          } else if(msg.find("m") != string::npos) {
              printf("Player %d sent message: %s\n",fds[i].fd, buffer);
              sendMessage();
          } else if(msg.find("r") != string::npos) { 
              printf("Restarting game!\n");
              restartGame();
          } else if(msg.find("e") != string::npos) {
              printf("Player %d didn't vote!\n");
          } else {
              printf("Player %d sent unrecognized string: %s\n",fds[i].fd, buffer);
          }

          if (rc < 0)
          {
            perror("  send() failed");
            // close_conn = TRUE;
            break;
          }


      }  // End of existing connection is readable
    } // End of loop through pollable descriptors
    if (compress_array)
    {
      compress_array = FALSE;
      for (i = 0; i < nfds; i++)
      {
        if (fds[i].fd == -1)
        {
          for(j = i; j < nfds; j++)
          {
            fds[j].fd = fds[j+1].fd;
          }
          nfds--;
        }
      }
    }

  } while (end_server == FALSE);
  for (i = 0; i < nfds; i++)
  {
    if(fds[i].fd >= 0)
      close(fds[i].fd);
  }
return 0;
}