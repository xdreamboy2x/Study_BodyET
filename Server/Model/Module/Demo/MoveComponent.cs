using System;
using System.Threading;
using UnityEngine;

namespace ETModel
{
    public class MoveComponent: Entity
    {
        public Vector3 Target;

        // 开启移动协程的时间
        public long StartTime;

        // 开启移动协程的Unit的位置
        public Vector3 StartPos;

        public long needTime;

        // 当前的移动速度
        public float Speed = 50;
        
        // 开启协程移动,每100毫秒移动一次，并且协程取消的时候会计算玩家真实移动
        // 比方说玩家移动了2500毫秒,玩家有新的目标,这时旧的移动协程结束,将计算250毫秒移动的位置，而不是300毫秒移动的位置
        public async ETTask StartMove(ETCancellationToken cancellationToken)
        {
            Log.Debug("StartMove");
            var transform = Parent.GetComponent<TransformComponent>();
            this.StartPos = transform.position;
            this.StartTime = TimeHelper.Now();
            float distance = (this.Target - this.StartPos).magnitude;
            if (Math.Abs(distance) < 0.1f)
            {
                return;
            }
            
            this.needTime = (long)(distance / this.Speed * 1000);
            
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();
            
            // 协程如果取消，将算出玩家的真实位置，赋值给玩家
            cancellationToken.Register(() =>
            {
                long timeNow = TimeHelper.Now();
                if (timeNow - this.StartTime >= this.needTime)
                {
                    transform.position = this.Target;
                }
                else
                {
                    float amount = (timeNow - this.StartTime) * 1f / this.needTime;
                    transform.position = Vector3.Lerp(this.StartPos, this.Target, amount);
                }
            });

            while (true)
            {
                await timerComponent.WaitAsync(50, cancellationToken);
                
                long timeNow = TimeHelper.Now();
                
                if (timeNow - this.StartTime >= this.needTime)
                {
                    transform.position = this.Target;
                    break;
                }

                float amount = (timeNow - this.StartTime) * 1f / this.needTime;
                transform.position = Vector3.Lerp(this.StartPos, this.Target, amount);
                //Log.Debug($"{Parent.Id} position={transform.position}");
            }
        }
        
        public async ETTask MoveToAsync(Vector3 target, ETCancellationToken cancellationToken)
        {
            var transform = Parent.GetComponent<TransformComponent>();
            Log.Debug($"MoveToAsync position={transform.position} target={target}");
            // 新目标点离旧目标点太近，不设置新的
            if ((target - this.Target).sqrMagnitude < 0.01f)
            {
                Log.Error($"新目标点离旧目标点太近，不设置新的 旧目标={this.Target} 新目标={target}");
                return;
            }

            // 距离当前位置太近
            if ((transform.position - target).sqrMagnitude < 0.01f)
            {
                Log.Error($"距离当前位置太近 当前位置={transform.position} 目标位置={target}");
                return;
            }
            
            this.Target = target;
            
            // 开启协程移动
            await StartMove(cancellationToken);
        }

        public async ETVoid MoveTo(Vector3 target, bool destroyWhenMoveEnd = false)
        {
            await MoveToAsync(target, EntityFactory.Create<ETCancellationTokenSource>(Domain).Token);
            if (destroyWhenMoveEnd)
            {
                if (Parent != null)
                    Parent.Dispose();
            }
        }
    }
}