/*
* Copyright (c) Contributors, http://www.openmetaverse.org/
* See CONTRIBUTORS.TXT for a full list of copyright holders.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenSim Project nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* 
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace libTerrain
{
    partial class Channel
    {
        enum NeighbourSystem
        {
            Moore,
            VonNeumann
        };

        private int[] Neighbours(NeighbourSystem type, int index)
        {
            int[] coord = new int[2];

            index++;

            switch (type)
            {
                case NeighbourSystem.Moore:
                    switch (index)
                    {
                        case 1:
                            coord[0] = -1;
                            coord[1] = -1;
                            break;

                        case 2:
                            coord[0] = -0;
                            coord[1] = -1;
                            break;

                        case 3:
                            coord[0] = +1;
                            coord[1] = -1;
                            break;

                        case 4:
                            coord[0] = -1;
                            coord[1] = -0;
                            break;

                        case 5:
                            coord[0] = -0;
                            coord[1] = -0;
                            break;

                        case 6:
                            coord[0] = +1;
                            coord[1] = -0;
                            break;

                        case 7:
                            coord[0] = -1;
                            coord[1] = +1;
                            break;

                        case 8:
                            coord[0] = -0;
                            coord[1] = +1;
                            break;

                        case 9:
                            coord[0] = +1;
                            coord[1] = +1;
                            break;

                        default:
                            break;
                    }
                    break;

                case NeighbourSystem.VonNeumann:
                    switch (index)
                    {
                        case 1:
                            coord[0] = 0;
                            coord[1] = -1;
                            break;

                        case 2:
                            coord[0] = -1;
                            coord[1] = 0;
                            break;

                        case 3:
                            coord[0] = +1;
                            coord[1] = 0;
                            break;

                        case 4:
                            coord[0] = 0;
                            coord[1] = +1;
                            break;

                        case 5:
                            coord[0] = -0;
                            coord[1] = -0;
                            break;

                        default:
                            break;
                    }
                    break;
            }

            return coord;
        }
    }
}
