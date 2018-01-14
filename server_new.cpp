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

string table = "vnnnnnnnnnN";

string msg;

void sendVote(int team){
    if(team){ //Send vote to Red Team
        for(map<int, string>::iterator it = red_sd.begin(); it != red_sd.end(); it++){
            table[10] = 'O';
            send(it->first, table.c_str(), 11, 0);
        }
    } else {
            for(map<int, string>::iterator it = blue_sd.begin(); it != blue_sd.end(); it++){
            table[10] = 'X';
            send(it->first, table.c_str(), 11, 0);
        }
    }
}

void setVote(int sd, char buf[11]){
    if(red_sd.count(sd) > 0){
        red_sd.at(sd) = string(buf, 11);
        printf("\033[1;31mRed player\033[0m %d voted for: %s\n", sd, red_sd.at(sd).c_str() );
        table = red_sd.at(sd);

        sendVote(0);
    }
    if(blue_sd.count(sd) > 0){
        blue_sd.at(sd) = string(buf, 11);
        printf("\033[1;34mBlue player\033[0m %d voted for: %s\n", sd, blue_sd.at(sd).c_str() );
        table = blue_sd.at(sd);

        sendVote(1);
    }
}

int main (int argc, char *argv[])
{
  int    len, rc, on = 1;
  int    listen_sd = -1, new_sd = -1;
  int    end_server = FALSE, compress_array = FALSE;
  int    close_conn;
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

  rc = setsockopt(listen_sd, SOL_SOCKET,  SO_REUSEADDR,
                  (char *)&on, sizeof(on));
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
  rc = bind(listen_sd,
            (struct sockaddr *)&addr, sizeof(addr));
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
			      int r = 0;
            red_sd.insert(pair<int, string>(new_sd, "nnnnnnnnn"));
			      r = send(fds[i].fd, "tXnnnnnnnnn", 12, 0);
            if (r != -1)
				      printf("New\033[1;31m red player\033[0m! \n");
			      else
				      printf("couldnt assign player to a team \n");
          } 
          else {
            int r = 0;
            blue_sd.insert(pair<int, string>(new_sd, "nnnnnnnnn"));
			      r = send(fds[i].fd, "tOnnnnnnnnn", 12, 0);
            if (r != -1)
				      printf("New\033[1;34m blue player\033[0;m! \n");
			      else
				      printf("couldnt assign player to a team \n");
          }

          nfds++;

        } while (new_sd != -1);
      }
      
      //Incoming message
      else
      {
        printf("  Descriptor %d is readable\n", fds[i].fd);
        close_conn = FALSE;
          rc = recv(fds[i].fd, buffer, sizeof(buffer), 0);
          if (rc < 0)
          {
            if (errno != EWOULDBLOCK)
            {
              perror("  recv() failed");
              close_conn = TRUE;
            }
            break;
          }
          //Check if client closed connection
          if (rc == 0)
          {
            printf("  Connection closed\n");
            close_conn = TRUE;
            goto conn;
            //Remove player TODO
          }

          len = rc;
          printf("  %d bytes received: %s\n", len, buffer);

          //Consume message
          msg = buffer;
          //Check if msg is a vote
          if(msg.find("v") != string::npos){
              setVote(fds[i].fd, buffer);
          }
          else if(msg.find("m") != string::npos){
              printf("Player %d sent message: %s\n",fds[i].fd, buffer);
          }
          else{
              printf("Player %d sent unrecognized string: %s\n",fds[i].fd, buffer);
          }

          rc = send(fds[i].fd, buffer, len, 0);
          if (rc < 0)
          {
            perror("  send() failed");
            close_conn = TRUE;
            break;
          }
        conn:
        if (close_conn)
        {
          close(fds[i].fd);
          fds[i].fd = -1;
          compress_array = TRUE;
        }


      }  /* End of existing connection is readable             */
    } /* End of loop through pollable descriptors              */

    /***********************************************************/
    /* If the compress_array flag was turned on, we need       */
    /* to squeeze together the array and decrement the number  */
    /* of file descriptors. We do not need to move back the    */
    /* events and revents fields because the events will always*/
    /* be POLLIN in this case, and revents is output.          */
    /***********************************************************/
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

  } while (end_server == FALSE); /* End of serving running.    */

  /*************************************************************/
  /* Clean up all of the sockets that are open                  */
  /*************************************************************/
  for (i = 0; i < nfds; i++)
  {
    if(fds[i].fd >= 0)
      close(fds[i].fd);
  }
return 0;
}