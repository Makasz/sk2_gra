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

using namespace std;

#define SERVER_PORT  12345

#define TRUE             1
#define FALSE            0

struct player{
    player(int sd){
        socket = sd;
    }
    int team;
    int socket;
    char vote[10] = "000000000";
};

struct game{
    char table[10] = "000000000";
    int red_cnt = 0;
    int blue_cnt = 0;
    int red_player[10];
    int blue_player[10];
    int r_pl_voted = 0;
    int b_pl_voted = 0;
};

struct game game_01;
map <int, struct player*> socket_to_id;
map <int, struct player*>::iterator it;

void print_player(int b){
    player* a = socket_to_id.find(b)->second;
    printf("    Team: %d\n    Socket: %d\n    Vote: %s\n---------\n", a->team, a->socket, a->vote);
}

void print_game(game g){
    printf("Table: %s\nRed count: %d\nBlue count: %d\n", g.table, g.red_cnt, g.blue_cnt);
    printf("Red players:\n");
    for(int i = 0; i<g.red_cnt; i++){
        print_player(g.red_player[i]);
    }
    printf("Blue players:\n");
    for(int i = 0; i<g.blue_cnt; i++){
        print_player(g.blue_player[i]);
    }
}

void update_table(int team){
    vector<char *> votes;
    if(team = 0){
        for(int i = 0; i < game_01.red_cnt; i++){
            it = socket_to_id.find(game_01.red_player[i]);
            votes.push_back(it->second->vote);
        }
    }
    if(team = 1){
        for(int i = 0; i < game_01.blue_cnt; i++){
            it = socket_to_id.find(game_01.blue_player[i]);
            votes.push_back(it->second->vote);
        }
    }
    for(auto i : votes){
        cout << i << endl;
    }
}

int main (int argc, char *argv[])
{
      int    len, rc, on = 1;
      int    listen_socekt = -1, new_sd = -1;
      int    end_server = FALSE, compress_array = FALSE;
      int    close_conn;
      char   buffer[10] = "999999999";
      struct sockaddr_in   addr;
      int    timeout;
      struct pollfd fds[200];
      int    nfds = 1, current_size = 0, i, j;

      listen_socekt = socket(AF_INET, SOCK_STREAM, 0); //Create socket
      if (listen_socekt < 0)
      {
        perror("socket() failed");
        exit(-1);
      }

      rc = setsockopt(listen_socekt, SOL_SOCKET,  SO_REUSEADDR,
                      (char *)&on, sizeof(on));
      if (rc < 0)
      {
        perror("setsockopt() failed");
        close(listen_socekt);
        exit(-1);
      }

      rc = ioctl(listen_socekt, FIONBIO, (char *)&on); //Set socket to nonblocking
      if (rc < 0)
      {
        perror("ioctl() failed");
        close(listen_socekt);
        exit(-1);
      }

      memset(&addr, 0, sizeof(addr)); //Binding socket
      addr.sin_family      = AF_INET;
      addr.sin_addr.s_addr = htonl(INADDR_ANY);
      addr.sin_port        = htons(SERVER_PORT);
      rc = bind(listen_socekt,
                (struct sockaddr *)&addr, sizeof(addr));
      if (rc < 0)
      {
        perror("bind() failed");
        close(listen_socekt);
        exit(-1);
      }

      rc = listen(listen_socekt, 32); //Set the listen back log
      if (rc < 0)
      {
        perror("listen() failed");
        close(listen_socekt);
        exit(-1);
      }

      memset(fds, 0 , sizeof(fds)); //Initalize pollfd

      fds[0].fd = listen_socekt; //Listening socket decalration
      fds[0].events = POLLIN;

      timeout = (3 * 60 * 1000);
  do //Waiting for incomign data/connections
  {
    printf("Waiting on poll()...\n"); //Poll invocation
    rc = poll(fds, nfds, timeout);

    if (rc < 0){ //Poll error
        perror("Poll() error");
        break;
    }
    if (rc == 0){ //Poll error
        perror("Poll() timeout");
        break;
    }
    current_size = nfds;
    for (i = 0; i < current_size; i++){
        if(fds[i].revents == 0){
            continue;
        }
      if(fds[i].revents != POLLIN) //Wrong activity
      {
        printf("  Error! revents = %d\n", fds[i].revents);
        end_server = TRUE;
        break;

      }
      cout << i << ": " << fds[i].fd << endl;
      if (fds[i].fd == listen_socekt) //Socket readeable
      {
        printf("  Listening socket is readable\n");
        do
        {
          new_sd = accept(listen_socekt, NULL, NULL);
          if (new_sd < 0)
          {
            if (errno != EWOULDBLOCK) //EWOULDBLOCK means we accepted all connections
            {
              perror("  accept() failed");
              end_server = TRUE;
            }
            break;
          }
          printf("  New incoming connection - %d\n", new_sd); //Add new connection
          fds[nfds].fd = new_sd;
          fds[nfds].events = POLLIN;

          if(game_01.red_cnt <= game_01.blue_cnt){ //Add player to smaller team
              socket_to_id[new_sd] = new player(new_sd); //Add new player to map
              socket_to_id.find(new_sd)->second->team = 0;//Set proper team to new player
              game_01.red_player[game_01.red_cnt] = new_sd;
              game_01.red_cnt++; //Update red player count
          } else {
              socket_to_id[new_sd] = new player(new_sd); //Add new player to map
              socket_to_id.find(new_sd)->second->team = 1;//Set proper team to new player
              game_01.blue_player[game_01.blue_cnt] = new_sd;
              game_01.blue_cnt++;
          }
          printf("Red team: %d   Blue team: %d\n", game_01.red_cnt, game_01.blue_cnt);

          nfds++;

        } while (new_sd != -1);
      }
      else //Descriptor readable
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

          if (rc == 0)
          {
            printf("  Connection closed\n");
            close_conn = TRUE;
            break;
          }

          len = rc;
          printf("  %d bytes received: %s\n", len, buffer);
          auto fds_cpy = fds[i].fd;

          if(!strcmp(buffer, "playerlist")){ //Return list of online players
              for_each(socket_to_id.begin(),socket_to_id.end(), [fds_cpy](pair<int, player*> i){
                  printf("%s\n", to_string(i.first).c_str());
                  send(fds_cpy, to_string(i.first).c_str(), sizeof(to_string(i.first)), 0);
              });
              continue;
          }

          it = socket_to_id.find(fds[i].fd); //Create iterator on wanted player
          copy(begin(buffer), end(buffer), begin(it->second->vote));
          if(it->second->team == 0){ //Send players vote to entire team (self included)
              for(int j = 0; j < game_01.red_cnt; j++){
                  rc = send(game_01.red_player[j], it->second->vote, sizeof(it->second->vote), 0);
                  game_01.r_pl_voted++;
                  if(game_01.r_pl_voted == game_01.red_cnt){ //All red palyer voted
                      rc = send(game_01.red_player[j], "redvoted ", sizeof("redvoted "), 0);
                      //Call finished voting handler TODO
                  }
              }
              if(game_01.r_pl_voted == game_01.red_cnt) game_01.r_pl_voted = 0;

          } else {
              for(int j = 0; j < game_01.blue_cnt; j++){
                  rc = send(game_01.blue_player[j], it->second->vote, sizeof(it->second->vote), 0);
                  game_01.b_pl_voted++;
                  if(game_01.b_pl_voted == game_01.blue_cnt){ //All blue palyer voted
                      rc = send(game_01.blue_player[j], "bluevoted", sizeof("bluevoted"), 0);
                      //Call finished voting handler TODO
                      game_01.b_pl_voted = 0;
                  }
              }
              if(game_01.b_pl_voted == game_01.blue_cnt) game_01.b_pl_voted = 0;
          }

          print_game(game_01);

          if (rc < 0)
          {
            perror("  send() failed");
            close_conn = TRUE;
            break;
          }
          fill_n(buffer, 80, 0);


        /*******************************************************/
        /* If the close_conn flag was turned on, we need       */
        /* to clean up this active connection. This           */
        /* clean up process includes removing the              */
        /* descriptor.                                         */
        /*******************************************************/
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

  for (i = 0; i < nfds; i++)
  {
    if(fds[i].fd >= 0)
      close(fds[i].fd);
  }
  return 0;
}
